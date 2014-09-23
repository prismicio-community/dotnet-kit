namespace prismic
open System
open System.Collections
open System.Collections.Generic

type TupleList<'a, 'b when 'a : equality>(inner: ('a * 'b) list)  =
    interface IEnumerable<'a * 'b> with
        member this.GetEnumerator() = (this.Inner |> List.toSeq).GetEnumerator()
    interface IEnumerable with
        override this.GetEnumerator() = (this.Inner |> List.toSeq).GetEnumerator() :> IEnumerator
    member x.Inner = inner
    member x.Item with get(key) = inner |> List.find (fun i -> fst i = key) |> snd

/// Signature file for the Util module
module TupleList =

    let toTupleList list = TupleList(list)
    let empty<'a, 'b when 'a : equality> = (TupleList<'a, 'b>([ ]))
    let rev (list:TupleList<'a, 'b>) = list.Inner |> List.rev |> toTupleList
    let values (predicate: 'a -> 'b -> bool) (list:TupleList<'a, 'b>) =
        list.Inner |> List.filter (fun (a, b) -> predicate a b) |> List.map (fun (a, b) -> b)
    let allkeys (list:TupleList<'a, 'b>) =
        list.Inner |> List.map fst
    let value (predicate: 'a -> 'b -> bool) (list:TupleList<'a, 'b>) =
        list.Inner |> List.tryFind (fun (a, b) -> predicate a b) |> Option.map snd
    let allvalues (list:TupleList<'a, 'b>) =
        list.Inner |> List.map snd
    let valueForKey key = value (fun k _ -> k = key) 
    let mapKeys (mapper:'a -> 'c) (list:TupleList<'a, 'b>) =
        list.Inner |> List.map (fun (a, b) -> (mapper a, b)) |> toTupleList
    let mapValues (mapper:'b -> 'c) (list:TupleList<'a, 'b>) =
        list.Inner |> List.map (fun (a, b) -> (a, mapper b)) |> toTupleList
    let ofSeq sequence = 
         sequence |> List.ofSeq |> toTupleList
    let toMap (list:TupleList<'a, 'b>) = 
         list.Inner |> Map.ofList
    let fold (folder:'s->('a*'b) ->'s) (state:'s) (list:TupleList<'a, 'b>) = 
        list.Inner 
        |> List.fold (fun s (a, b) -> folder s (a, b) ) state
    let add (k, v) (list:TupleList<'a, 'b>) = (k, v) :: list.Inner |> toTupleList
    let set (k, v) (list:TupleList<'a, 'b>) = 
        (k, v) :: (list.Inner |> List.filter(fun (a, b) -> a <> k)) |> toTupleList

