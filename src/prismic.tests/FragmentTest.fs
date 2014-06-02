namespace prismic.tests
open prismic
open System
open NUnit.Framework
open Fragments
open FragmentsGetters
open FragmentsHtml

module FragmentTest =

    let await a = a |> Async.RunSynchronously
    let apiGetNoCache = Api.get (ApiInfra.NoCache() :> ApiInfra.ICache<Api.Response>) (ApiInfra.Logger.NoLogger)

    [<TestFixture>]
    type ``Query Document and Parse Fragments``() = 

        [<Test>]
        member x.``Should Access Group Field``() = 
            let url = "https://micro.prismic.io/api"
            let api = await (apiGetNoCache (Option.None) url)
            let form = api.Forms.["everything"].Ref(api.Master).Query("""[[:d = at(document.type, "docchapter")]]""")
            let document = await(form.Submit()).results |> List.head 
            let maybeGroup = document.fragments |> getGroup "docchapter.docs"
            match maybeGroup with
                            | Some(Group(docs)) -> 
                                match docs |> Seq.tryPick Some with
                                    | Some(doc) -> match doc.fragments |> getLink "linktodoc" with
                                                            | Some(Link(DocumentLink(l))) -> Assert.IsNotNull(l.id)
                                                            | _ -> Assert.Fail("No link found")
                                    | _ -> Assert.Fail("No document found")
                            | _ -> Assert.Fail("Result is not of type group")


        [<Test>]
        member x.``Should Serialize Group To HTML``() = 
            let url = "https://micro.prismic.io/api"
            let api = await (apiGetNoCache (Option.None) url)
            let form = api.Forms.["everything"].Ref(api.Master).Query("""[[:d = at(document.type, "docchapter")]]""")
            let documentf = await(form.Submit()).results 
            let document = List.nth documentf  1
            let maybeGroup = document.fragments |> getGroup "docchapter.docs"
            match maybeGroup with
                            | Some(g) -> 
                                let linkresolver = Api.DocumentLinkResolver.For(fun l -> 
                                    String.Format("""http://localhost/{0}/{1}""", l.typ, l.id))
                                let html = Api.asHtml linkresolver g
                                Assert.AreEqual("""<section data-field="linktodoc"><a href="http://localhost/doc/UrDejAEAAFwMyrW9">installing-meta-micro</a></section>
<section data-field="desc"><p>Just testing another field in a group section.</p></section>
<section data-field="linktodoc"><a href="http://localhost/doc/UrDmKgEAALwMyrXA">using-meta-micro</a></section>""", html)
                            | _ -> Assert.Fail("Result is not of type group")


        [<Test>]
        member x.``Should Serialize Another Group To HTML``() = 
            let url = "https://micro.prismic.io/api"
            let api = await (apiGetNoCache (Option.None) url)
            let form = api.Forms.["everything"].Ref(api.Master).Query("""[[:d = at(document.type, "docchapter")]]""")
            let documentf = await(form.Submit()).results 
            let document = List.nth documentf  0
            let maybeGroup = document.fragments |> getGroup "docchapter.docs"
            match maybeGroup with
                            | Some(g) -> 
                                let linkresolver = Api.DocumentLinkResolver.For(fun l -> 
                                    String.Format("""http://localhost/{0}/{1}""", l.typ, l.id))
                                let html = Api.asHtml linkresolver g
                                Assert.AreEqual("""<section data-field="linktodoc"><a href="http://localhost/doc/UrDofwEAALAdpbNH">with-jquery</a></section>
<section data-field="linktodoc"><a href="http://localhost/doc/UrDp8AEAAPUdpbNL">with-bootstrap</a></section>""", html)
                            | _ -> Assert.Fail("Result is not of type group")



        [<Test>]
        member x.``Should Access Media Link``() = 
            let url = "https://test-public.prismic.io/api"
            let api = await (apiGetNoCache (Option.None) url)
            let form = api.Forms.["everything"].Ref(api.Master).Query("""[[:d = at(document.id, "Uyr9_wEAAKYARDMV")]]""")
            let document = await(form.Submit()).results |> List.head 
            let maybeLink = document.fragments |> getLink "test-link.related"
            match maybeLink with
                            | Some(Link(MediaLink(l))) -> Assert.AreEqual("baastad.pdf", l.filename)
                            | _ -> Assert.Fail("Media Link not found")

        [<Test>]
        member x.``Should Access First Link In Multiple Document Link``() = 
            let url = "https://lesbonneschoses.prismic.io/api"
            let api = await (apiGetNoCache (Option.None) url)
            let form = api.Forms.["everything"].Ref(api.Master).Query("""[[:d = at(document.id, "UkL0gMuvzYUANCpi")]]""")
            let document = await(form.Submit()).results |> List.head 
            let maybeLink = document.fragments |> getLink "job-offer.location"
            match maybeLink with
                            | Some(Link(DocumentLink(l))) -> Assert.AreEqual("new-york-fifth-avenue", l.slug)
                            | _ -> Assert.Fail("Document Link not found")


        [<Test>]
        member x.``Should Find All Links In Multiple Document Link``() = 
            let url = "https://lesbonneschoses.prismic.io/api"
            let api = await (apiGetNoCache (Option.None) url)
            let form = api.Forms.["everything"].Ref(api.Master).Query("""[[:d = at(document.id, "UkL0gMuvzYUANCpi")]]""")
            let document = await(form.Submit()).results |> List.head 
            let links = document.fragments |> getAll "job-offer.location"
            Assert.AreEqual(5, links |> Seq.length)
            let link0 = links |> Seq.nth 0
            match link0 with
                            | Link(DocumentLink(l)) -> Assert.AreEqual("new-york-fifth-avenue", l.slug)
                            | _ -> Assert.Fail("Document Link not found")
            let link1 = links |> Seq.nth 1
            match link1 with
                            | Link(DocumentLink(l)) -> Assert.AreEqual("tokyo-roppongi-hills", l.slug)
                            | _ -> Assert.Fail("Document Link not found")


        [<Test>]
        member x.``Should Access Structured Text``() = 
            let url = "https://lesbonneschoses.prismic.io/api"
            let api = await (apiGetNoCache (Option.None) url)
            let form = api.Forms.["everything"].Ref(api.Master).Query("""[[:d = at(document.id, "UkL0gMuvzYUANCpu")]]""")
            let document = await(form.Submit()).results |> List.head 
            let maybeStructTxt = document.fragments |> getStructuredText "article.content"
            Assert.IsTrue(maybeStructTxt.IsSome)

        [<Test>]
        member x.``Should Access Image``() = 
            let url = "https://test-public.prismic.io/api"
            let api = await (apiGetNoCache (Option.None) url)
            let form = api.Forms.["everything"].Ref(api.Master).Query("""[[:d = at(document.id, "Uyr9sgEAAGVHNoFZ")]]""")
            let document = await(form.Submit()).results |> List.head 
            let maybeImg = document.fragments |> getImageView "article.illustration" "icon"

            match maybeImg with
                            | Some(_) -> 
                                let expectpattern = """<img alt="some alt text" src="{0}" width="100" height="100" />"""
                                let url = "https://prismic-io.s3.amazonaws.com/test-public/9f5f4e8a5d95c7259108e9cfdde953b5e60dcbb6.jpg"
                                let expect = String.Format(expectpattern, url)
                                let image = imageViewAsHtml maybeImg.Value
                                Assert.AreEqual(expect, image)
                            | _ -> Assert.Fail("Document Link not found")


