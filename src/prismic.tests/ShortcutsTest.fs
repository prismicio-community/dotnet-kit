namespace prismic.tests
open System
open NUnit.Framework

[<TestFixture>]
type ShortcutTest() = 

    [<Test>]
    member x.CanConvertUnixTime() =
        let e = prismic.Shortcuts.fromUnixTimeMs 1405468800000L
        Console.WriteLine(e.ToString("yyyy-MM-dd"))
        Assert.AreEqual(2014, e.Year)
        Assert.AreEqual(7, e.Month)
        Assert.AreEqual(16, e.Day)

