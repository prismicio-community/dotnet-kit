namespace prismic.tests
open System
open NUnit.Framework
open FSharp.Data
open prismic


[<TestFixture>]
type SpanParsingTest() =

    let structured_text_paragraph = """{
  "type": "StructuredText",
  "value": [{
    "type": "paragraph",
    "text": "Experience the ultimate vanilla experience. Our vanilla Macarons are made with our very own (in-house) pure extract of Madagascar vanilla, and subtly dusted with our own vanilla sugar (which we make from real vanilla beans).",
    "spans": [
      {
      "start": 103,
      "end": 137,
      "type": "em"
    },
    {
      "start": 11,
      "end": 14,
      "type": "hyperlink",
      "data": {
        "type": "Link.web",
        "value": {
          "url": "http://prismic.io"
        }
      }
    },
    {
      "start": 162,
      "end": 183,
      "type": "strong"
    },
    {
      "start": 24,
      "end": 31,
      "type": "label",
      "data": {
        "label": "flavour"
      }
    }
    ]}
  ]
}"""
    let structured_text_with_tricky_spans = """{
    "type": "StructuredText",
    "value" : [
        {
            "type" : "heading3",
            "text" : "Powering Through 2013 ",
            "spans" : [
                {
                    "start": 0,
                    "end": 22,
                    "type": "strong"
                }
            ]
        },
        {
            "type" : "heading3",
            "text" : "Online Resources:",
            "spans" : [
                {
                    "start": 0,
                    "end": 17,
                    "type": "strong"
                }
            ]
        },
        {
            "type" : "list-item",
            "text" : "Hear more from our executive team as they reflect on 2013 and share their vision for 2014 on our blog here",
            "spans" : [
                {
                    "start" : 102,
                    "end" : 106,
                    "type" : "hyperlink",
                    "data" : {
                        "type" : "Link.web",
                        "value" : {
                            "url" : "http://prismic.io"
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
    member x.``Should Serialize Span Html In structured Texts when end is at the end of line``() =
        let json = JsonValue.Parse structured_text_with_tricky_spans
        let fragment = FragmentsParsers.parseFragment(json)
        let expected = """<h3><strong>Powering Through 2013 </strong></h3>
<h3><strong>Online Resources:</strong></h3>
<ul>
<li>Hear more from our executive team as they reflect on 2013 and share their vision for 2014 on our blog <a href="http://prismic.io">here</a></li>
</ul>"""
        let actual = fragment |> Option.map (fun f -> Api.asHtml linkresolver f)
        Assert.AreEqual(Some(expected), actual)


    [<Test>]
    member x.``Should Serialize Span Html in structured texts when multiple spans``() =
        let json = JsonValue.Parse structured_text_paragraph
        let fragment = FragmentsParsers.parseFragment(json)
        let expected = """<p>Experience <a href="http://prismic.io">the</a> ultimate <span class="flavour">vanilla</span> experience. Our vanilla Macarons are made with our very own (in-house) <em>pure extract of Madagascar vanilla</em>, and subtly dusted with <strong>our own vanilla sugar</strong> (which we make from real vanilla beans).</p>"""
        let actual = fragment |> Option.map (fun f -> Api.asHtml linkresolver f)
        Assert.AreEqual(Some(expected), actual)
