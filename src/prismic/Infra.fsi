namespace prismic
open FSharp.Data

module Infra =

    [<Interface>]
    type ICache<'a> =
        abstract member Set: string -> 'a -> System.DateTimeOffset -> unit
        abstract member Get: string -> 'a option

    type NoCache<'a> = 
        new : unit -> NoCache<'a>
        interface ICache<'a> 


    type Logger =
        new: unit -> Logger 
        static member NoLogger: (string -> string -> unit)
