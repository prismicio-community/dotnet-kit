namespace prismic
open System.Collections
open System.Collections.Generic

type TupleList<'a,'b when 'a : equality> =
    class
        new : inner:('a * 'b) list -> TupleList<'a,'b>
        interface IEnumerable<'a * 'b>
        interface IEnumerable
        member Inner : ('a * 'b) list
        member Item : key:'a -> 'b with get
    end

module TupleList =

    val add :
        k:'a * v:'b -> list:TupleList<'a,'b> -> TupleList<'a,'b>
        when 'a : equality
    val empty<'a,'b when 'a : equality> :
        TupleList<'a,'b>
        when 'a : equality
    val rev :
        list:TupleList<'a,'b> -> TupleList<'a,'b>
        when 'a : equality
    val allkeys : list:TupleList<'a,'b> -> 'a list
        when 'a : equality
    val allvalues : list:TupleList<'a,'b> -> 'b list
        when 'a : equality
    val values :
        predicate:('a -> 'b -> bool) -> list:TupleList<'a,'b> -> 'b list
        when 'a : equality
    val value :
        predicate:('a -> 'b -> bool) -> list:TupleList<'a,'b> -> 'b option
        when 'a : equality
    val valueForKey :
        key:'a -> (TupleList<'a,'b> -> 'b option)
        when 'a : equality
    val mapValues :
        mapper:('b -> 'c) -> list:TupleList<'a,'b> -> TupleList<'a,'c>
        when 'a : equality
    val mapKeys :
        mapper:('a -> 'c) -> list:TupleList<'a,'b> -> TupleList<'c,'b>
        when 'a : equality and 'c : equality
    val ofSeq :
        sequence:seq<'a * 'b> -> TupleList<'a,'b>
        when 'a : equality
    val toMap : list:TupleList<'a,'b> -> Map<'a,'b>
        when 'a : comparison
    val fold :
        folder:('s -> 'a * 'b -> 's) -> state:'s -> list:TupleList<'a,'b> -> 's
        when 'a : equality
    val set :
        k:'a * v:'b -> list:TupleList<'a,'b> -> TupleList<'a,'b>
        when 'a : equality
