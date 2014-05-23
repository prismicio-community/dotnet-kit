namespace prismic
open FSharp.Data

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



    type DocumentLinkResolver =
        new : f:(Fragments.DocumentLink -> string) -> DocumentLinkResolver
        member Apply : documentLink:Fragments.DocumentLink -> string
        static member For : f:(Fragments.DocumentLink -> string) -> DocumentLinkResolver


    type Ref = {
        ref: string;
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
        fields: Map<string,Field>; }
    with
        member defaultData : Map<string,seq<string>>
    end


    type ApiData = {
        refs: seq<Ref>;
        bookmarks: Map<string,string>;
        types: Map<string,string>;
        tags: seq<string>;
        forms: Map<string,Form>;
        oauthEndpoints: string * string; }


    type Document = {
        id: string;
        typ: string;
        href: string;
        tags: seq<string>;
        slugs: seq<string>;
        fragments: Map<string,Fragments.Fragment>;}
    with
        member isTagged : (seq<string> -> bool)
        member slug : string
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
        new : form:Form * values:Map<string, string seq> * cache:prismic.Infra.ICache<Response> * logger:(string->string->unit) -> SearchForm
        member Orderings : o:string -> SearchForm
        member Page : p:int -> SearchForm
        member PageSize : p:int -> SearchForm
        member Query : q:string -> SearchForm
        member Ref : value:string -> SearchForm
        member Ref : value:Ref -> SearchForm
        member Set : fieldName:string * value:string -> SearchForm
        member Set : fieldName:string * value:int -> SearchForm
        member Submit : unit -> Async<Response>

    type Api =
        new : data:ApiData * cache:prismic.Infra.ICache<Response> * logger:(string->string->unit) -> Api
        member Bookmarks : Map<string, string>
        member Forms : Map<string, SearchForm>
        member Master : Ref
        member OauthInitiateEndpoint : string
        member OauthTokenEndpoint : string
        member Refs : Map<string, Ref>

    /// <summary>Fetches a response from the api for the given url and returns an Api.</summary>
    /// <param name="cache">Caches the responses according to their max-age.</param>
    /// <param name="logger">Logs the queries.</param>
    /// <param name="token">auth token.</param>
    /// <param name="url">url of the prismic repository.</param>
    /// <returns>an API.</returns>
    /// <exception cref="prismic.FetchingException">Thrown when the response fails to be retreived.</exception>
    /// <exception cref="prismic.ParsingException">Thrown when the response fails to be parsed.</exception>
    val get : cache:prismic.Infra.ICache<Response> -> logger: (string -> string -> unit) -> token:string option -> url:string -> Async<Api>

    /// <summary>Make HTML for a Fragement.</summary>
    /// <param name="linkResolver">Resolves the links within the document.</param>
    /// <param name="fragment">Fragment to process.</param>
    /// <returns>The HTML.</returns>
    val asHtml : linkResolver:DocumentLinkResolver -> (Fragments.Fragment -> string)


