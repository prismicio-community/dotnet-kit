namespace prismic

[<Interface>]
type Cache =
    abstract member Set: string -> 'a -> unit
    abstract member Get: string -> 'a option

type NoCache = 
    new : unit -> NoCache