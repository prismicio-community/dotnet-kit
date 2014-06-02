namespace prismic

/// Signature file for the ApiInfra module
module ApiInfra =

    /// Cache interface, allows to provide a caching strategy for Api Responses 
    [<Interface>]
    type ICache<'a> =
        /// key, value, expiration time (now + max-age)
        abstract member Set: string -> 'a -> System.DateTimeOffset -> unit
        abstract member Get: string -> 'a option
    
    /// Can be used when no cache is wanted
    type NoCache<'a> = 
        new : unit -> NoCache<'a>
        interface ICache<'a> 

    /// Provides a no logger 
    type Logger =
        new: unit -> Logger 
        static member NoLogger: (string -> string -> unit)
