namespace prismic.tests
open System
open NUnit.Framework
open prismic
open FSharp.Data
open FragmentsGetters
open FSharp.Data.JsonExtensions

[<TestFixture>]
type GroupParsingTest() = 

    let group = """{
                        "type": "Group", 
                        "value": [
                            {
                                "desc": {
                                    "type": "StructuredText", 
                                    "value": [
                                        {
                                            "spans": [], 
                                            "text": "Just testing another field in a group section.", 
                                            "type": "paragraph"
                                        }
                                    ]
                                }, 
                                "linktodoc": {
                                    "type": "Link.document", 
                                    "value": {
                                        "document": {
                                            "id": "UrDejAEAAFwMyrW9", 
                                            "slug": "installing-meta-micro", 
                                            "tags": [], 
                                            "type": "doc"
                                        }, 
                                        "isBroken": false
                                    }
                                }, 
                                "linktodoc2": {
                                    "type": "Link.document", 
                                    "value": {
                                        "document": {
                                            "id": "UrDejAEAAFwMyr00", 
                                            "slug": "aa--desinstalling-meta-micro", 
                                            "tags": [], 
                                            "type": "doc"
                                        }, 
                                        "isBroken": false
                                    }
                                }, 
                                "linktodoc3": {
                                    "type": "Link.document", 
                                    "value": {
                                        "document": {
                                            "id": "00DejAEAAFwMyr00", 
                                            "slug": "bb--desinstalling-meta-micro", 
                                            "tags": [], 
                                            "type": "doc"
                                        }, 
                                        "isBroken": false
                                    }
                                }, 
                                "linktodoc4": {
                                    "type": "Link.document", 
                                    "value": {
                                        "document": {
                                            "id": "000000EAAFwMyr00", 
                                            "slug": "cc--desinstalling-meta-micro", 
                                            "tags": [], 
                                            "type": "doc"
                                        }, 
                                        "isBroken": false
                                    }
                                }, 
                                "linktodoc5": {
                                    "type": "Link.document", 
                                    "value": {
                                        "document": {
                                            "id": "00000000AFwMyr00", 
                                            "slug": "dd--desinstalling-meta-micro", 
                                            "tags": [], 
                                            "type": "doc"
                                        }, 
                                        "isBroken": false
                                    }
                                }
                            }, 
                            {
                                "linktodoc": {
                                    "type": "Link.document", 
                                    "value": {
                                        "document": {
                                            "id": "UrDmKgEAALwMyrXA", 
                                            "slug": "using-meta-micro", 
                                            "tags": [], 
                                            "type": "doc"
                                        }, 
                                        "isBroken": false
                                    }
                                }
                            }
                        ]
                    }"""


    [<Test>]
    member x.``Can Parse Group And preserve order``() =
        let group = JsonValue.Parse group
        let fragment = FragmentsParsers.parseFragment(group)
        match fragment with
                | Some(Fragments.Group(docs)) -> 
                    match docs |> Seq.toList with
                        | first :: second :: [] -> 
                            Assert.AreEqual(6, first.fragments.Inner |> List.length)
                            Assert.AreEqual("desc", List.nth first.fragments.Inner 0 |> fst)
                            Assert.AreEqual("linktodoc", List.nth first.fragments.Inner 1 |> fst)
                            Assert.AreEqual("linktodoc2", List.nth first.fragments.Inner 2 |> fst)
                            Assert.AreEqual("linktodoc3", List.nth first.fragments.Inner 3 |> fst)
                            Assert.AreEqual("linktodoc4", List.nth first.fragments.Inner 4 |> fst)
                            Assert.AreEqual("linktodoc5", List.nth first.fragments.Inner 5 |> fst)
                            Assert.AreEqual(1, second.fragments.Inner |> List.length)
                            Assert.AreEqual("linktodoc", List.nth second.fragments.Inner 0 |> fst)
                            Assert.IsTrue(true)
                        | _ -> Assert.Fail("Supposed to find 2 docs in the group.")
                | _ -> Assert.Fail("Not a group. A group should have been parsed and it's not a group")

