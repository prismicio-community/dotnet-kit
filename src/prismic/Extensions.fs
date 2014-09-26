namespace prismic.extensions
open System.Runtime.CompilerServices
open FSharp.Data
open prismic
open prismic.FragmentsGetters

    /// adapters for C#

    [<Extension>]
    type CSharpAdapters() =


       [<Extension>] 
       static member ToFSharpFunc<'a,'b> (func:System.Converter<'a,'b>) = fun x -> func.Invoke(x)

       [<Extension>] 
       static member ToFSharpFunc<'a,'b> (func:System.Func<'a,'b>) = fun x -> func.Invoke(x)

       [<Extension>] 
       static member ToFSharpFunc<'a,'b,'c> (func:System.Func<'a,'b,'c>) = fun x y -> func.Invoke(x,y)

       [<Extension>] 
       static member ToFSharpFunc<'a,'b,'c,'d> (func:System.Func<'a,'b,'c,'d>) = fun x y z -> func.Invoke(x,y,z)

       [<Extension>] 
       static member ToFSharpFunc<'a,'b> (func:System.Action<'a,'b>) = fun x y -> func.Invoke(x,y) |> ignore

       static member CreateFunc<'a,'b> (func:System.Func<'a,'b>) = CSharpAdapters.ToFSharpFunc func

       static member CreateFunc<'a,'b,'c> (func:System.Func<'a,'b,'c>) = CSharpAdapters.ToFSharpFunc func

       static member CreateFunc<'a,'b,'c,'d> (func:System.Func<'a,'b,'c,'d>) = CSharpAdapters.ToFSharpFunc func


       [<Extension>]
       static member Exists(opt : 'a option) =
                            match opt with
                              | Some _ -> true
                              | None -> false
       [<Extension>]
       static member Value(opt : 'a option) =
                            match opt with
                              | Some v -> v
                              | None -> raise (System.NullReferenceException("option has no value. Please check Exists before trying to get the value out of an option."))

       [<Extension>]
       static member Map(opt : 'a option, mapper:System.Func<'a,'b>) = opt |> Option.map (CSharpAdapters.CreateFunc(mapper))

       [<Extension>]
       static member Bind(opt : 'a option, binder:System.Func<'a, 'b option>) = opt |> Option.bind (CSharpAdapters.CreateFunc(binder))

       [<Extension>]
       static member GetOrElse(opt : 'a option, elseValue) = match opt with Some(v) -> v | None -> elseValue



