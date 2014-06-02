namespace prismic.tests
open System
open NUnit.Framework
open prismic

[<TestFixture>]
type TupleListTest() = 

    [<Test>]
    member x.CanSetValue() =
        let init = TupleList([("a", "b"); ("z", "y")])
        let set2 (k, v) (list:TupleList<'a, 'b>) = TupleList(list.Inner |> List.map (fun (a, b) -> 
                                                        (a, if a=k then v else b) ))
        let set = init |> set2 ("a","c") 
        Assert.AreEqual("c", set.["a"])
        Assert.AreEqual("y", set.["z"])




