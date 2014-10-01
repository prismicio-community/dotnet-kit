namespace prismic.tests
open System
open NUnit.Framework
open FSharp.Data
open prismic

[<TestFixture>]
type LinkParsingTest() =

    let structured_text_linkfile = """{
  "type": "StructuredText",
  "value": [
    {
      "type": "paragraph",
      "text": "2012 Annual Report",
      "spans": [
        {
          "start": 0,
          "end": 18,
          "type": "hyperlink",
          "data": {
            "type": "Link.file",
            "value": {
              "file": {
                "name": "2012_annual.report.pdf",
                "kind": "document",
                "url": "https://prismic-io.s3.amazonaws.com/annual.report.pdf",
                "size": "1282484"
              }
            }
          }
        }
      ]
    },
    {
      "type": "paragraph",
      "text": "2012 Annual Budget",
      "spans": [
        {
          "start": 0,
          "end": 18,
          "type": "hyperlink",
          "data": {
            "type": "Link.file",
            "value": {
              "file": {
                "name": "2012_smec.annual.budget.pdf",
                "kind": "document",
                "url": "https://prismic-io.s3.amazonaws.com/annual.budget.pdf",
                "size": "59229"
              }
            }
          }
        }
      ]
    },
    {
      "type": "paragraph",
      "text": "2015 Vision & Strategic Plan",
      "spans": [
        {
          "start": 0,
          "end": 28,
          "type": "hyperlink",
          "data": {
            "type": "Link.file",
            "value": {
              "file": {
                "name": "2015_vision.strategic.plan_.sm_.pdf",
                "kind": "document",
                "url": "https://prismic-io.s3.amazonaws.com/vision.strategic.plan_.sm_.pdf",
                "size": "1969956"
              }
            }
          }
        }
      ]
    }
  ]
}"""

    let linkresolver = Api.DocumentLinkResolver.For(fun l ->
                        String.Format("""http://localhost/{0}/{1}""", l.typ, l.id))


    [<Test>]
    member x.``Should Serialize MediaLink Html In structured Texts``() =
        let json = JsonValue.Parse structured_text_linkfile
        let fragment = FragmentsParsers.parseFragment(json)
        let expected = """<p><a href="https://prismic-io.s3.amazonaws.com/annual.report.pdf">2012 Annual Report</a></p>
<p><a href="https://prismic-io.s3.amazonaws.com/annual.budget.pdf">2012 Annual Budget</a></p>
<p><a href="https://prismic-io.s3.amazonaws.com/vision.strategic.plan_.sm_.pdf">2015 Vision &amp; Strategic Plan</a></p>"""
        let actual = fragment |> Option.map (fun f -> Api.asHtml linkresolver Api.HtmlSerializer.Empty f) 
        Assert.AreEqual(Some(expected), actual)
