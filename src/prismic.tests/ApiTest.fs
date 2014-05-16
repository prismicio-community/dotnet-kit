namespace prismic.tests
open prismic
open System
open NUnit.Framework

module ApiTest =

    let await a = a |> Async.RunSynchronously
    let expectException statement matchesException = 
        try
            statement() |> ignore
            Assert.Fail("expected exception was not raised")
        with | e -> match matchesException(e) with
                    | true -> Assert.That(true)
                    | _ -> Assert.Fail(sprintf "unexpected type of exception happened: %s %s" (e.GetType().Name) e.Message)

    [<TestFixture>]
    type ``Get Private Api``() = 

        [<Test>]
        member x.``Without Authorization Token Should Throw``() =
            let url = "https://private-test.prismic.io/api"
            expectException 
                (fun () -> await (Api.get (Option.None) url))
                (function
                    | Api.AuthorizationNeeded(_, url) -> url = "https://private-test.prismic.io/auth"
                    | e -> false)

        [<Test>]
        member x.``With Invalid Token Should Throw``() =
            let url = "https://private-test.prismic.io/api"
            expectException 
                (fun () -> await (Api.get (Option.Some("dummy-token")) url))
                (function
                    | Api.InvalidToken(_, url) -> url = "https://private-test.prismic.io/auth"
                    | e -> false)


