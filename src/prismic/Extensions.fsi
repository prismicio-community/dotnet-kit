namespace prismic.extensions
open FSharp.Data
open System.Runtime.CompilerServices
open prismic

    /// adapters for C#

    [<Extension>]
    type CSharpAdapters =
        new : unit -> CSharpAdapters

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
 

        [<Extension>]
        static member Exists : opt: 'a option -> bool
        [<Extension>]
        static member Value : opt: 'a option -> 'a
        [<Extension>]
        static member Map : opt: 'a option * mapper:System.Func<'a,'b> -> 'b option

        [<Extension>]
        static member Bind : opt: 'a option * binder:System.Func<'a, 'b option> -> 'b option

        [<Extension>]
        static member GetOrElse : opt: 'a option * elseValue:'a -> 'a
