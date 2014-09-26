namespace prismic
open Microsoft.FSharp.Collections
open FSharp.Data
open FSharp.Data.JsonExtensions
open System.Net
open Shortcuts
open FragmentsParsers

module Api =

    type OAuthUrl = string

    exception AuthorizationNeeded of string * OAuthUrl
    exception InvalidToken of string * OAuthUrl
    exception UnexpectedError of string

    type FetchingException (message:string, ?innerException:exn) =
        inherit System.Exception (message, 
            match innerException with | Some(ex) -> ex | _ -> null)
    and ParsingException (message:string, ?innerException:exn) =
        inherit System.Exception (message, 
            match innerException with | Some(ex) -> ex | _ -> null)

    type Ref = { releaseId:string; refId:string; label:string; isMasterRef:bool; scheduledAt:System.DateTime option }
                       static member fromJson (json:JsonValue) = {
                            releaseId = json?id.AsString();
                            refId = json?ref.AsString(); 
                            label = json?label.AsString(); 
                            isMasterRef = (asBooleanOption(json>?"isMasterRef")) <?- false; 
                            scheduledAt = asDateTimeFromUnixMsOption(json>?"scheduledAt") 
                       }

    and Field = { fieldType: string; multiple: bool; fieldDefault: string option }
                        static member fromJson (json:JsonValue) = {
                            fieldType = json.GetProperty("type").AsString();
                            multiple = (asBooleanOption(json>?"multiple")) <?- false;
                            fieldDefault = asStringOption(json>?"default")
                        }

    and Form = { name:string option; formMethod:string; rel:string option; enctype:string; action:string; fields: TupleList<string, Field> } 
                        static member fromJson (json:JsonValue) = { 
                            name = asStringOption(json>?"name");
                            formMethod = json.GetProperty("method").AsString();
                            rel = asStringOption(json>?"rel");
                            enctype = json?enctype.AsString();
                            action = json?action.AsString();
                            fields = asTupleListFromProperties (Field.fromJson) (json?fields)
                        }
                        member this.defaultData = 
                            let addDefault state (k, f) = 
                                f.fieldDefault |> Option.fold (fun m defval -> m |> TupleList.add (k, seq { yield defval })) state
                            this.fields |> TupleList.fold addDefault TupleList.empty


    and ApiData = { refs:Ref seq; bookmarks:Map<string, string>; types:Map<string, string>; tags:string seq; forms: TupleList<string, Form>; oauthEndpoints:string * string  }
                        static member fromJson (json:JsonValue) = { 
                            refs = json?refs.AsArray() |> Array.map Ref.fromJson; 
                            bookmarks = asStringMapFromProperties(json?bookmarks);
                            types = asStringMapFromProperties(json?types);
                            tags = json?tags.AsArray() |> Array.map asString; 
                            forms = asTupleListFromProperties (Form.fromJson) (json?forms)
                            oauthEndpoints = (json?oauth_initiate.AsString(), json?oauth_token.AsString())
                        }

    and LinkedDocument = { id: string; typ:string; slug: string option; tags: string seq }
                            static member fromJson (json:JsonValue) = {
                                id = json?id.AsString();
                                typ = json.GetProperty("type").AsString();
                                slug = asStringOption(json>?"slug")
                                tags = json?tags.AsArray() |> Array.map asString; 
                            }

    and Document = { id: string; typ: string; href: string; tags: string seq; slugs: string seq; 
                        fragments: TupleList<string, Fragments.Fragment>; linkedDocuments: LinkedDocument seq }
                        member this.slug = this.slugs |> Seq.tryPick Some <?- "-"
                        member this.isTagged = Seq.forall (fun t -> this.tags |> Seq.exists ((=) t))
                        static member fromJson (json:JsonValue) = 
                            let dType = json.GetProperty("type").AsString() in

                            let parseFragmentsField ((typ:string), j) = 
                                match parseField j with
                                    | Some(ParsedField.Single(fragment))  
                                        ->  Seq.singleton (System.String.Format("{0}.{1}", dType, typ), fragment)
                                    | Some(ParsedField.Array(elements)) 
                                        -> elements |> Seq.map (fun (idx, fragment) 
                                                                    -> System.String.Format("{0}.{1}[{2}]", dType, typ, idx), fragment)
                                    | None -> Seq.empty
                            
                            let fragments = json?data.GetProperty(dType).Properties |> Seq.ofArray
                                            |> Seq.collect parseFragmentsField |> TupleList.ofSeq

                            {
                                id = json?id.AsString();
                                href = json?href.AsString();
                                tags = json?tags.AsArray() |> Array.map asString; 
                                slugs = json?slugs.AsArray() |> Array.map asString;
                                typ = dType;
                                fragments = fragments;
                                linkedDocuments = asArrayOrEmpty json "linked_documents" |> Array.map LinkedDocument.fromJson |> List.ofArray; 
                            }

    and Response = { results: List<Document>; page: int; resultsPerPage:int; resultsSize:int; totalResultsSize:int; totalPages:int; nextPage: string option; prevPage: string option; }
                        static member fromJson (json:JsonValue) = { 
                                results = json?results.AsArray() |> Array.map Document.fromJson |> List.ofArray; 
                                page = json?page.AsInteger();
                                resultsPerPage = json?results_per_page.AsInteger();
                                resultsSize = json?results_size.AsInteger();
                                totalResultsSize = json?total_results_size.AsInteger();
                                totalPages = json?total_pages.AsInteger();
                                nextPage = asStringOption(json>?"next_page");
                                prevPage = asStringOption(json>?"prev_page");
                            }

    type SearchForm(form, values:TupleList<string, string seq>, cache:prismic.ApiInfra.ICache<Response>, logger) = 
        let tryFindField fieldName = form.fields |> TupleList.valueForKey fieldName <?-- (lazy raise (System.ArgumentException(sprintf "unknown field %s" fieldName)))
        let (<<=) (field,fieldName) value = 
            let singleOrAppend _ v = if field.multiple then Seq.append v (Seq.singleton value) else Seq.singleton value 
            let fieldValues = values |> TupleList.valueForKey fieldName |> Option.fold singleOrAppend (Seq.singleton value)
            let newvalues = values |> TupleList.set (fieldName,  fieldValues)
            SearchForm(form, newvalues, cache, logger)
        member this.Set(fieldName, value:string) = 
            let f = tryFindField fieldName
            (f,fieldName) <<= value
        member this.Set(fieldName, value:int) = 
            let f = tryFindField fieldName
            if f.fieldType <> "Integer" then raise (System.ArgumentException(sprintf "Cannot use a Int as value for the field %s of type %s" fieldName f.fieldType))
            (f,fieldName) <<= (value.ToString())
        member this.Ref(refId:string) = this.Set("ref", refId)
        member this.Ref(value:Ref) = this.Ref(value.refId)
        member this.Query(q:string) = 
            match form.fields |> TupleList.valueForKey "q" |> Option.map (fun field -> field.multiple) with 
                | Some(true) -> this.Set("q", q)
                | Some(false)
                | _ ->  (*let strip (q:string) = 
                                    let a = q.Trim().ToCharArray() 
                                    System.String(Array.sub a 1 (a.Length - 2))
                        *)
                        this.Set("q", q)  // Add the Temporary Hack for backward compatibility ?
        member this.Page(p:int) = this.Set("page", p)
        member this.PageSize(p:int) = this.Set("pageSize", p) 
        member this.Orderings(o:string) = this.Set("orderings", o) 
        member this.Submit() = 
            async {
                match (form.formMethod, form.enctype, form.action) with
                    | ("GET", "application/x-www-form-urlencoded", action) 
                        ->  let url = ApiCore.buildUrl action (values |> TupleList.toMap)
                            match cache.Get url.PathAndQuery with
                                | Some(cached) -> return cached
                                | None -> 
                                    let request = HttpWebRequest.Create(url) :?> HttpWebRequest
                                    request.UserAgent <- ApiCore.userAgent
//                                  request.AllowAutoRedirect <- true
//                                  request.ReadWriteTimeout <- 120000
//                                  request.Timeout <- 120000
                                    request.Accept <- "application/json"

                                    let tryFetch req = async {
                                        try
                                            return! ApiCore.fetch req logger
                                        with | e -> return raise (FetchingException(sprintf "Got an error while fetching url %s" (url.ToString()), e))
                                    }

                                    let tryParse p = 
                                        try 
                                            let parsed = JsonValue.Parse p
                                            Response.fromJson parsed
                                        with 
                                            | :? ParsingException -> reraise()
                                            | e ->  raise(ParsingException(sprintf "Exception during parsing of %s" p, e))

                                    let tryMatchMaxAge = function
                                        | ParseRegex "max-age\s*=\s*(\d+)" d -> Some(d)
                                        | _ -> None

                                    let addToCache (parsed:Response) (response:ApiCore.httpResponse) = 
                                            match response.headers |> Map.tryFind "Cache-Control" |> Option.bind (fun a -> a |> Array.tryPick tryMatchMaxAge) with
                                                | Some(maxage :: []) -> 
                                                    let expiration = System.Int64.Parse(maxage) |> float
                                                    cache.Set url.PathAndQuery parsed (System.DateTimeOffset.Now.AddSeconds(expiration))
                                                    parsed
                                                | _ -> parsed

                                    let! fetched = tryFetch(request)
                                    match fetched.statusCode with
                                        | HttpStatusCode.OK ->  return addToCache (tryParse fetched.body) fetched
                                        | _ -> return raise (UnexpectedError(sprintf "Got an unexpected HTTP status %s (%s)" (fetched.statusCode.ToString()) fetched.statusText))
                    | _ -> return raise (System.ArgumentException(sprintf "Form type not supported"))
            }


    and Api(data:ApiData, cache:prismic.ApiInfra.ICache<Response>, logger:string->string->unit) =
        member this.Refs = data.refs  
                                    |> Seq.groupBy (fun r -> r.label)
                                    |> Seq.fold (fun (m:Map<string, Ref>) (k, t) -> m.Add(k, Seq.head t)) Map.empty
        member this.Bookmarks = data.bookmarks
        member this.Forms = data.forms |> TupleList.mapValues (fun form -> SearchForm(form, form.defaultData, cache, logger))
        member this.Master = data.refs |> Seq.tryFind (fun r -> r.isMasterRef) <?-- (lazy raise(ParsingException("no master reference found")))
        member this.OauthInitiateEndpoint = fst data.oauthEndpoints
        member this.OauthTokenEndpoint = snd data.oauthEndpoints
        static member fromJson j cache logger =
                    let tryParse p = 
                        try 
                            let parsed = JsonValue.Parse p
                            ApiData.fromJson parsed 
                        with 
                            | :? ParsingException -> reraise()
                            | e -> raise(ParsingException(sprintf "Exception during parsing of %s" j, e))
                    let apidata = tryParse j
                    Api(apidata, cache, logger)

    
    /// <summary>Builds URL specific to an application, based on a generic prismic.io document link.</summary>
    type DocumentLinkResolver(f) = 
        /// <summary>Builds a document link resolver that will apply the function to document links.
        /// For C# users, there is an adapter, see prismic.extensions.DocumentLinkResolver</summary>
        /// <param name="f">the resolving strategy.</param>
        /// <returns>a DocumentLinkResolver for the given strategy.</returns>
        static member For(f:Fragments.DocumentLink -> string) = DocumentLinkResolver(f)
        /// <summary>Builds a document link resolver that will apply the function to document links and bookmark.
        /// For C# users, there is an adapter, see prismic.extensions.DocumentLinkResolver</summary>
        /// <param name="api">the api.</param>
        /// <param name="f">the resolving strategy, on document link and evenually a bookmark.</param>
        /// <returns>a DocumentLinkResolver for the given strategy.</returns>
        static member For(api:Api, f:Fragments.DocumentLink -> string option -> string) =
            DocumentLinkResolver(fun link -> f link (api.Bookmarks.TryFind(link.id)))
        member this.Apply(documentLink) = f(documentLink)
        member this.Apply(document:Document) = f({ id = document.id;
                                                   typ = document.typ;
                                                   tags = document.tags;
                                                   slug = document.slug;
                                                   isBroken = false; })


    let selectFromJson j map = 
           let parsed = JsonValue.Parse j
           map(parsed)



    let fetchPrismicJson cache logger (url:string) (token:string option) = async {
        let request = HttpWebRequest.Create(url) :?> HttpWebRequest
        request.UserAgent <- ApiCore.userAgent
        request.AllowAutoRedirect <- true
        request.ReadWriteTimeout <- 120000
        request.Timeout <- 120000
        request.Accept <- "application/json"

        let tryFetch req = async {
            try
                return! ApiCore.fetch req logger
            with | e -> return raise (FetchingException(sprintf "Got an error while fetching url %s" url, e))
        }

        let! fetched = tryFetch(request)
        match fetched.statusCode with
            | HttpStatusCode.OK -> return Api.fromJson fetched.body cache logger
            | HttpStatusCode.Unauthorized -> 
                let oauthurl = selectFromJson fetched.body (fun p -> p.TryGetProperty "oauth_initiate")
                match oauthurl with
                    | Some(u) when token.IsNone -> return raise (AuthorizationNeeded("You need to provide an access token to access this repository", u.AsString()))
                    | Some(u) -> return raise (InvalidToken("The provided access token is either invalid or expired", u.AsString()))
                    | None -> return raise (UnexpectedError("Authorization error, but no URL was provided"))
            | _ -> return raise (UnexpectedError(sprintf "Got an unexpected HTTP status %s (%s)" (fetched.statusCode.ToString()) fetched.statusText))
    }

    /// <summary>Fetches a response from the api for the given url and returns an Api.</summary>
    /// <param name="cache">Caches the responses according to their max-age.</param>
    /// <param name="logger">Logs the queries.</param>
    /// <param name="token">auth token.</param>
    /// <param name="url">url of the prismic repository.</param>
    /// <returns>an API.</returns>
    /// <exception cref="prismic.FetchingException">Thrown when the response fails to be retreived.</exception>
    /// <exception cref="prismic.ParsingException">Thrown when the response fails to be parsed.</exception>
    let get cache logger token url = async {
        let urlOptToken = token |> Option.map (fun t -> System.String.Format("{0}?access_token={1}", url, t)) <?- url
        let! api = fetchPrismicJson cache logger urlOptToken token
        return api
    }

    /// <summary>Make HTML for a Fragement.</summary>
    /// <param name="linkResolver">Resolves the links within the document.</param>
    /// <param name="fragment">Fragment to process.</param>
    /// <returns>The HTML.</returns>
    let asHtml (linkResolver:DocumentLinkResolver) = FragmentsHtml.asHtml linkResolver.Apply

    /// <summary>Make HTML for a Document.</summary>
    /// <param name="linkResolver">Resolves the links within the document.</param>
    /// <param name="document">Document to process.</param>
    /// <returns>The HTML.</returns>
    let documentAsHtml (linkResolver:DocumentLinkResolver) (document:Document) =
        let asGroupDoc = ({Fragments.GroupDoc.fragments = document.fragments}) |> Seq.singleton
        FragmentsHtml.asHtml linkResolver.Apply (Fragments.Group(asGroupDoc))


