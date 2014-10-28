namespace prismic
open FSharp.Data
open Experiments

/// Signature file for the Api module
module Api =
    type OAuthUrl = string

    exception AuthorizationNeeded of string * OAuthUrl
    exception InvalidToken of string * OAuthUrl
    exception UnexpectedError of string

    type FetchingException =
        inherit System.Exception
        new : message:string * ?innerException:exn -> FetchingException
    and ParsingException =
        inherit System.Exception
        new : message:string * ?innerException:exn -> ParsingException


    type Ref = {
        releaseId : string;
        refId: string;
        label: string;
        isMasterRef: bool;
        scheduledAt: System.DateTime option; }


    type Field = {
        fieldType: string;
        multiple: bool;
        fieldDefault: string option; }


    type Form = {
        name: string option;
        formMethod: string;
        rel: string option;
        enctype: string;
        action: string;
        fields: TupleList<string, Field>; }
    with
        member defaultData : TupleList<string,seq<string>>
    end


    type ApiData = {
        refs: seq<Ref>;
        bookmarks: Map<string,string>;
        types: Map<string,string>;
        tags: seq<string>;
        forms: TupleList<string, Form>;
        experiments: Experiments;
        oauthEndpoints: string * string; }

    type LinkedDocument = { id: string; typ:string; slug: string option; tags: string seq }

    type Document = {
        id: string;
        typ: string;
        href: string;
        tags: seq<string>;
        slugs: seq<string>;
        fragments: TupleList<string, Fragments.Fragment>;
        linkedDocuments: LinkedDocument seq}
    with
        member isTagged : (seq<string> -> bool)
        member slug : string
        static member fromJson : JsonValue -> Document
    end

    type Response = {
            results: List<Document>;
            page: int; resultsPerPage:int;
            resultsSize:int;
            totalResultsSize:int;
            totalPages:int;
            nextPage: string option;
            prevPage: string option }

    type SearchForm =
        new : form:Form * values:TupleList<string, string seq> * cache:prismic.ApiInfra.ICache<Response> * logger:(string->string->unit) -> SearchForm
        member Orderings : o:string -> SearchForm
        member Page : p:int -> SearchForm
        member PageSize : p:int -> SearchForm
        member Query : q:string -> SearchForm
        member Ref : refId:string -> SearchForm
        member Ref : value:Ref -> SearchForm
        member Set : fieldName:string * value:string -> SearchForm
        member Set : fieldName:string * value:int -> SearchForm
        member Submit : unit -> Async<Response>

    type Api =
        new : data:ApiData * cache:prismic.ApiInfra.ICache<Response> * logger:(string->string->unit) -> Api
        member Bookmarks : Map<string, string>
        member Forms : TupleList<string, SearchForm>
        member Master : Ref
        member OauthInitiateEndpoint : string
        member OauthTokenEndpoint : string
        member Refs : Map<string, Ref>

    /// <summary>Builds URL specific to an application, based on a generic prismic.io document link.</summary>
    type DocumentLinkResolver =
        new : f:(Fragments.DocumentLink -> string) -> DocumentLinkResolver
        member Apply : documentLink:Fragments.DocumentLink -> string
        member Apply : document:Document -> string
        /// <summary>Builds a document link resolver that will apply the function to document links.
        /// For C# users, there is an adapter, see prismic.extensions.DocumentLinkResolver</summary>
        /// <param name="f">the resolving strategy.</param>
        /// <returns>a DocumentLinkResolver for the given strategy.</returns>
        static member For : f:(Fragments.DocumentLink -> string) -> DocumentLinkResolver
        /// <summary>Builds a document link resolver that will apply the function to document links and bookmark.
        /// For C# users, there is an adapter, see prismic.extensions.DocumentLinkResolver</summary>
        /// <param name="api">the api.</param>
        /// <param name="f">the resolving strategy, on document link and evenually a bookmark.</param>
        /// <returns>a DocumentLinkResolver for the given strategy.</returns>
        static member For : api:Api * f:(Fragments.DocumentLink -> string option -> string) -> DocumentLinkResolver

    type HtmlSerializer =
        new: f:(Fragments.Element -> string -> string option) -> HtmlSerializer
        static member Empty: HtmlSerializer
        static member For: f:(Fragments.Element -> string -> string option) -> HtmlSerializer
        // For C# compatibility
        static member For: f:(System.Object -> string -> string) -> HtmlSerializer
        member Apply: Fragments.Element -> string -> string option

    /// <summary>Fetches a response from the api for the given url and returns an Api.</summary>
    /// <param name="cache">Caches the responses according to their max-age.</param>
    /// <param name="logger">Logs the queries.</param>
    /// <param name="token">auth token.</param>
    /// <param name="url">url of the prismic repository.</param>
    /// <returns>an API.</returns>
    /// <exception cref="prismic.FetchingException">Thrown when the response fails to be retreived.</exception>
    /// <exception cref="prismic.ParsingException">Thrown when the response fails to be parsed.</exception>
    val get : cache:prismic.ApiInfra.ICache<Response> -> logger: (string -> string -> unit) -> token:string option -> url:string -> Async<Api>

    /// <summary>Make HTML for a Fragement.</summary>
    /// <param name="linkResolver">Resolves the links within the document.</param>
    /// <param name="fragment">Fragment to process.</param>
    /// <returns>The HTML.</returns>
    val asHtml : linkResolver:DocumentLinkResolver -> htmlSerializer:HtmlSerializer -> (Fragments.Fragment -> string)

    /// <summary>Make HTML for a Document.</summary>
    /// <param name="linkResolver">Resolves the links within the document.</param>
    /// <param name="document">Document to process.</param>
    /// <returns>The HTML.</returns>
    val documentAsHtml : linkResolver:DocumentLinkResolver -> htmlSerializer:HtmlSerializer -> document:Document -> string
