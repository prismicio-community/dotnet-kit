namespace prismic
//open System.Runtime.Caching

type Cache =
    abstract member Set: string -> 'a -> unit
    abstract member Get: string -> 'a option
   

type NoCache () =
    //let cache = MemoryCache.Default
   interface Cache with
        member this.Set key value = ()
        member this.Get key = None
