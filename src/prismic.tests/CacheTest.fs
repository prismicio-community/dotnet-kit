namespace prismic.tests
open System
open prismic
open prismic.ApiInfra
open NUnit.Framework

[<TestFixture>]
type SimpleCacheTest() = 

    let fill (cache:ICache<string>) nbEntries = 
        for i in [1..nbEntries] do cache.Set ("foo"+i.ToString()) ("val"+i.ToString()) (DateTimeOffset.Now.AddMinutes(10.))
    
    [<Test>]
    member x.GetNotExistingEntryReturnsNone() =
        let cache = new SimpleCache<string>(10) :> ICache<string>
        let foo = cache.Get "/foo"
        Assert.IsTrue(foo.IsNone)

    [<Test>]
    member x.CanSetAndGetEntries() =
        let cache = new SimpleCache<string>(10) :> ICache<string>
        cache.Set "/foo" "fooVal" (DateTimeOffset.Now.AddMinutes(10.))
        cache.Set "/bar" "barVal" (DateTimeOffset.Now.AddMinutes(10.))

        let foo = cache.Get "/foo"
        Assert.IsTrue(foo.IsSome)
        Assert.AreEqual("fooVal", foo.Value)

        let bar = cache.Get "/bar"
        Assert.IsTrue(bar.IsSome)
        Assert.AreEqual("barVal", bar.Value)

    [<Test>]
    member x.SetAnExistingEntryOverwritesIt() =
        let cache = new SimpleCache<string>(10) :> ICache<string>
        cache.Set "/foo" "fooVal" (DateTimeOffset.Now.AddMinutes(10.))
        cache.Set "/foo" "newFooVal" (DateTimeOffset.Now.AddMinutes(10.))

        let foo = cache.Get "/foo"
        Assert.IsTrue(foo.IsSome)
        Assert.AreEqual("newFooVal", foo.Value)


    [<Test>]
    member x.ShouldDiscardExpiredEntries() =
        let cache = new SimpleCache<string>(10) :> ICache<string>
        cache.Set "/foo" "fooVal" (DateTimeOffset.Now.AddMilliseconds(400.))

        System.Threading.Thread.Sleep(401) |> ignore
        let foo = cache.Get "/foo"
        Assert.IsFalse(foo.IsSome)

    [<Test>]
    member x.ShouldDiscardShortlyExpiredEntriesWhenCacheCapacityIsReached() =
        let cache = new SimpleCache<string>(5) :> ICache<string>
        cache.Set "/foo1" "foo1" (DateTimeOffset.Now.AddMinutes(1.))
        cache.Set "/foo2" "foo2" (DateTimeOffset.Now.AddMinutes(2.))
        cache.Set "/foo3" "foo3" (DateTimeOffset.Now.AddMinutes(3.))
        cache.Set "/foo4" "foo4" (DateTimeOffset.Now.AddMinutes(4.))
        cache.Set "/foo5" "foo5" (DateTimeOffset.Now.AddMinutes(5.))

        let foo = cache.Get "/foo1"
        Assert.IsTrue(foo.IsSome)

        cache.Set "/foo6" "foo6" (DateTimeOffset.Now.AddMinutes(6.))

        let foo = cache.Get "/foo1"
        Assert.IsFalse(foo.IsSome)
