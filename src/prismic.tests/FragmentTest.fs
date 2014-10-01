namespace prismic.tests
open prismic
open System
open FSharp.Data
open NUnit.Framework
open Fragments
open FragmentsGetters
open FragmentsHtml

module FragmentTest =

    let await a = a |> Async.RunSynchronously
    let apiGetNoCache = Api.get (ApiInfra.NoCache() :> ApiInfra.ICache<Api.Response>) (ApiInfra.Logger.NoLogger)
    let htmlSerializer = Api.HtmlSerializer.Empty

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
                                let html = Api.asHtml linkresolver htmlSerializer g
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
                                let html = Api.asHtml linkresolver htmlSerializer g
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
            let form = api.Forms.["everything"].Ref(api.Master).Query("""[[:d = at(document.id, "UlfoxUnM0wkXYXba")]]""")
            let document = await(form.Submit()).results |> List.head
            let maybeLink = document.fragments |> getLink "job-offer.location"
            match maybeLink with
                            | Some(Link(DocumentLink(l))) -> Assert.AreEqual("paris-saint-lazare", l.slug)
                            | _ -> Assert.Fail("Document Link not found")


        [<Test>]
        member x.``Should Find All Links In Multiple Document Link``() =
            let url = "https://lesbonneschoses.prismic.io/api"
            let api = await (apiGetNoCache (Option.None) url)
            let form = api.Forms.["everything"].Ref(api.Master).Query("""[[:d = at(document.id, "UlfoxUnM0wkXYXba")]]""")
            let document = await(form.Submit()).results |> List.head
            let links = document.fragments |> getAll "job-offer.location"
            Assert.AreEqual(3, links |> Seq.length)
            let link0 = links |> Seq.nth 0
            match link0 with
                            | Link(DocumentLink(l)) -> Assert.AreEqual("paris-saint-lazare", l.slug)
                            | _ -> Assert.Fail("Document Link not found")
            let link1 = links |> Seq.nth 1
            match link1 with
                            | Link(DocumentLink(l)) -> Assert.AreEqual("tokyo-roppongi-hills", l.slug)
                            | _ -> Assert.Fail("Document Link not found")


        [<Test>]
        member x.``Should Access Structured Text``() =
            let url = "https://lesbonneschoses.prismic.io/api"
            let api = await (apiGetNoCache (Option.None) url)
            let form = api.Forms.["everything"].Ref(api.Master).Query("""[[:d = at(document.id, "UlfoxUnM0wkXYXbX")]]""")
            let document = await(form.Submit()).results |> List.head
            let maybeStructTxt = document.fragments |> getStructuredText "blog-post.body"
            Assert.IsTrue(maybeStructTxt.IsSome)

        [<Test>]
        member x.``Should Access Image``() =
            let linkresolver = fun (l: DocumentLink) -> String.Format("""http://localhost/{0}/{1}""", l.typ, l.id)
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
                                let image = imageViewAsHtml linkresolver maybeImg.Value
                                Assert.AreEqual(expect, image)
                            | _ -> Assert.Fail("Document Link not found")


        [<Test>]
        member x.``Should Access GeoPoint``() =
            let url = "https://test-public.prismic.io/api"
            let api = await (apiGetNoCache (Option.None) url)
            let form = api.Forms.["everything"].Ref(api.Master).Query("""[[:d = at(document.id, "U9pZMDQAADEAYj_n")]]""")
            let document = await(form.Submit()).results |> List.head
            let maybeGeoPoint = document.fragments |> getGeoPoint "product.location"

            match maybeGeoPoint with
                            | Some(g) ->
                                let expected = """<div class="geopoint"><span class="latitude">48.87687670000001</span><span class="longitude">2.3338801999999825</span></div>"""
                                let linkresolver = Api.DocumentLinkResolver.For(fun l -> String.Format("""http://localhost/{0}/{1}""", l.typ, l.id))
                                Assert.AreEqual(expected, Api.asHtml linkresolver htmlSerializer g)
                            | _ -> Assert.Fail("GeoPoint not found")

        [<Test>]
        member x.``Should Access Embed``() =
            let url = "https://test-public.prismic.io/api"
            let api = await (apiGetNoCache (Option.None) url)
            let form = api.Forms.["everything"].Ref(api.Master).Query("""[[:d = at(document.id, "Uy4VGQEAAPQzRDR9")]]""")
            let document = await(form.Submit()).results |> List.head
            let maybeEmbed = document.fragments |> getEmbed "test-link.embed"

            match maybeEmbed with
                            | Some(e) ->
                                let expected = """<div data-oembed="https://gist.github.com/srenault/71b4f1e62783c158f8af" data-oembed-type="rich" data-oembed-provider="github"><script src="https://gist.github.com/srenault/71b4f1e62783c158f8af.js"></script></div>"""
                                let linkresolver = Api.DocumentLinkResolver.For(fun l -> String.Format("""http://localhost/{0}/{1}""", l.typ, l.id))
                                Assert.AreEqual(expected, Api.asHtml linkresolver htmlSerializer e)
                            | _ -> Assert.Fail("Ebed not found")

        [<Test>]
        member x.``Shoud Parse Timestamp``() =
            let json = JsonValue.Parse "{\"id\": \"UlfoxUnM0wkXYXbm\",\
                \"type\": \"blog-post\",\
                \"href\": \"https://example.com\",\
                \"slugs\": [\"tips-to-dress-a-pastry\"],\
                \"tags\": [],\
                \"data\": { \"blog-post\": { \"when\": { \"type\": \"Timestamp\", \"value\": \"2014-06-18T15:30:00+0000\" } } } }"
            let document = prismic.Api.Document.fromJson (json)
            let tstamp = document.fragments |> getTimestamp "blog-post.when"
            let linkresolver = Api.DocumentLinkResolver.For(fun l -> String.Format("""http://localhost/{0}/{1}""", l.typ, l.id))

            match tstamp with
                    | Some(ts) ->
                        Assert.AreEqual("<time>2014-06-18 15:30:00</time>", Api.asHtml linkresolver htmlSerializer ts)
                    | _ -> Assert.Fail("Timestamp not found")
