namespace prismic.extensions
open FSharp.Data
open System.Runtime.CompilerServices
open prismic

    /// adapters for C#

    [<Extension>]
    type CoreEx =
        new : unit -> CoreEx
        [<Extension>]
        static member Exists : opt: 'a option -> bool
        [<Extension>]
        static member Value : opt: 'a option -> 'a
        [<Extension>]
        static member ToFSharpFunc : func:System.Converter<'a,'b> -> ('a -> 'b)
        [<Extension>]
        static member ToFSharpFunc : func:System.Func<'a,'b> -> ('a -> 'b)
        [<Extension>]
        static member ToFSharpFunc : func:System.Func<'a,'b,'c> -> ('a -> 'b -> 'c)
        [<Extension>]
        static member ToFSharpFunc : func:System.Func<'a,'b,'c,'d> -> ('a -> 'b -> 'c -> 'd)
        [<Extension>]
        static member ToFSharpFunc : func:System.Action<'a,'b> -> ('a -> 'b -> unit)

        static member CreateFunc : func:System.Func<'a,'b> -> ('a -> 'b)
        static member CreateFunc : func:System.Func<'a,'b,'c> -> ('a -> 'b -> 'c)
        static member CreateFunc : func:System.Func<'a,'b,'c,'d> -> ('a -> 'b -> 'c -> 'd)
 