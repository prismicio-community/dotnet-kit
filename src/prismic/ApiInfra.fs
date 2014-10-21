namespace prismic
open FSharp.Data
open System
open System.Collections.Concurrent
open System.Linq


module ApiInfra =

    /// Provides a no logger 
    type Logger() = 
        static member NoLogger = fun (level:string) (message:string) -> ()

    /// Cache interface, allows to provide a caching strategy for Api Responses 
    type ICache<'a> =
        /// key, value, expiration time (now + max-age)
        abstract member Set: string -> 'a-> System.DateTimeOffset -> unit
        abstract member Get: string -> 'a option

    type Cache<'a>(set: string -> 'a -> System.DateTimeOffset -> unit, get: string -> ('a option)) =
        interface ICache<'a> with
            member this.Set key value expiration = (set key value expiration)
            member this.Get key = (get key)

    /// Can be used when no cache is wanted
    type NoCache<'a> () =
       interface ICache<'a> with
            member this.Set key value expiration = ()
            member this.Get key = None

    /// Very simple cache with a capacity. Will trim entries that expire soon.
    type SimpleCache<'a>
        private (cache : ConcurrentDictionary<string, 'a * int64>,
                 capacity : int) =
        let trimBound = 
                let fcapacity = float capacity
                int (System.Math.Round(fcapacity - (fcapacity * 0.1)))
        
        let addOrUpdate key value expiry = 
            let entry = (value, expiry)
            let update _ _ = entry
            cache.AddOrUpdate(key, entry, update) |> ignore

        new (capacity) = SimpleCache(new ConcurrentDictionary<string, 'a * int64>(), capacity)

        interface ICache<'a> with
            member this.Set key value expiration = 
                addOrUpdate key value expiration.Ticks
                if cache.Count > capacity then
                    for entry in cache.OrderByDescending(fun e -> let (_, expiry) = e.Value
                                                                  expiry)
                                      .Skip(trimBound) do
                        cache.TryRemove(entry.Key) |> ignore

            member this.Get key = 
                let now = DateTimeOffset.Now.Ticks
                match cache.TryGetValue(key) with
                    | (true, (value, expiry)) when now < expiry -> 
                        addOrUpdate key value expiry
                        Some(value)
                    | (true, _) -> 
                        cache.TryRemove(key) |> ignore
                        None
                    | (false, _) -> None


