namespace prismic.extensions
open System.Runtime.CompilerServices
open FSharp.Data
open prismic
open prismic.FragmentsGetters



    [<Extension>]
    type CoreEx() =

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
       static member ToFSharpFunc<'a,'b> (func:System.Converter<'a,'b>) = fun x -> func.Invoke(x)

       [<Extension>] 
       static member ToFSharpFunc<'a,'b> (func:System.Func<'a,'b>) = fun x -> func.Invoke(x)

       [<Extension>] 
       static member ToFSharpFunc<'a,'b,'c> (func:System.Func<'a,'b,'c>) = fun x y -> func.Invoke(x,y)

       [<Extension>] 
       static member ToFSharpFunc<'a,'b,'c,'d> (func:System.Func<'a,'b,'c,'d>) = fun x y z -> func.Invoke(x,y,z)

       static member CreateFunc<'a,'b> (func:System.Func<'a,'b>) = CoreEx.ToFSharpFunc func

       static member CreateFunc<'a,'b,'c> (func:System.Func<'a,'b,'c>) = CoreEx.ToFSharpFunc func

       static member CreateFunc<'a,'b,'c,'d> (func:System.Func<'a,'b,'c,'d>) = CoreEx.ToFSharpFunc func

     
