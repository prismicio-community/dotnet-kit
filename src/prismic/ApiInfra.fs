namespace prismic
open FSharp.Data

module ApiInfra =

    /// Cache interface, allows to provide a caching strategy for Api Responses 
    type ICache<'a> =
        /// key, value, expiration time (now + max-age)
        abstract member Set: string -> 'a-> System.DateTimeOffset -> unit
        abstract member Get: string -> 'a option
       
    /// Can be used when no cache is wanted
    type NoCache<'a> () =
       interface ICache<'a> with
            member this.Set key value expiration = ()
            member this.Get key = None


    /// Provides a no logger 
    type Logger() = 
        static member NoLogger = fun (level:string) (message:string) -> ()

