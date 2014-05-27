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
                    | _ -> reraise()//Assert.Fail(sprintf "unexpected type of exception happened: %s %s" (e.GetType().Name) e.Message)
    
    let apiGetNoCache = Api.get (ApiInfra.NoCache() :> ApiInfra.ICache<Api.Response>) (ApiInfra.Logger.NoLogger)

    [<TestFixture>]
    type ``Get Private Api``() = 

        [<Test>]
        member x.``Without Authorization Token Should Throw``() =
            let url = "https://private-test.prismic.io/api"
            expectException 
                (fun () -> await (apiGetNoCache (Option.None) url))
                (function
                    | Api.AuthorizationNeeded(_, url) -> url = "https://private-test.prismic.io/auth"
                    | e -> false)

        [<Test>]
        member x.``With Invalid Token Should Throw``() =
            let url = "https://private-test.prismic.io/api"
            expectException 
                (fun () -> await (apiGetNoCache (Option.Some("dummy-token")) url))
                (function
                    | Api.InvalidToken(_, url) -> url = "https://private-test.prismic.io/auth"
                    | e -> false)


