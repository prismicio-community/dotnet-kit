namespace prismic
open FSharp.Data

module Infra =
    type ICache<'a> =
        abstract member Set: string -> 'a-> System.DateTimeOffset -> unit
        abstract member Get: string -> 'a option
       

    type NoCache<'a> () =
       interface ICache<'a> with
            member this.Set key value expiration = ()
            member this.Get key = None



    type Logger() = 
        static member NoLogger = fun (level:string) (message:string) -> ()

