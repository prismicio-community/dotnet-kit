namespace prismic
open System
open Fragments
open Shortcuts

module FragmentsGetters = 


    let getTitle = function
                        StructuredText(blocks) 
                            -> blocks 
                                |> Seq.tryPick (function Block.Text(Text.Heading(_))&Block.Text(heading) -> Some(heading)
                                                                                        | _ -> None)
                        | _ -> None

    let getFirstImage = function
                        StructuredText(blocks) 
                            -> blocks 
                                |> Seq.tryPick (function Block.Image(v)&image -> Some(image)
                                                                                    | _ -> None)
                        | _ -> None

    let getFirstParagraph = function
                        StructuredText(blocks) 
                            -> blocks 
                                |> Seq.tryPick (function Block.Text(Text.Paragraph(_))&Block.Text(paragraph) -> Some(paragraph) 
                                                                                                                 | _ -> None)
                        | _ -> None

    let getAllParagraphs = function
                        StructuredText(blocks) 
                            -> blocks 
                                |> Seq.collect (function Block.Text(Text.Paragraph(_))&Block.Text(paragraph) -> Seq.singleton (paragraph)
                                                                                          | _ -> Seq.empty)
                        | _ -> Seq.empty


    let getAll field (fragmentMap:Map<string, Fragment>) = 
        let matchKey (a:string) =
            tryParseIndexedKey(a) |> Option.bind (fun k -> if k.Head = field then Some(k) else None)
        fragmentMap |> Map.filter (fun k v -> matchKey(k) |> Option.isSome) |> Map.toSeq |> Seq.map snd

    let get field (fragmentMap:Map<string, Fragment>) =
        let mayBeField = fragmentMap |> Map.tryFind field 
        match mayBeField with Some(_) -> mayBeField
                              | None -> getAll field fragmentMap |> Seq.tryPick Some

    let getLink field fragmentMap =
        get field fragmentMap 
            |> Option.bind (fun f -> match f with   
                                        | Link(Link.WebLink(_)) 
                                        | Link(Link.MediaLink(_))
                                        | Link(Link.DocumentLink(_)) -> Some(f)
                                        | _ -> None)

    let getImage field fragmentMap = 
        get field fragmentMap 
            |> Option.bind (fun f -> match f with   
                                        | Image(_) -> Some(f)
                                        | StructuredText(blocks) -> 
                                            blocks 
                                                |> Seq.tryPick (fun b -> match b with 
                                                                            | Block.Image(v) -> Some(Fragment.Image(v, Map.empty))
                                                                            | _ -> None)
                                        | _ -> None)

    let getAllImages field fragmentMap =
        getAll field fragmentMap 
            |> Seq.collect (fun f -> match f with   
                                        | Fragment.Image(_) -> Seq.singleton f
                                        | Fragment.StructuredText(blocks) -> 
                                            blocks 
                                                |> Seq.choose (fun b -> match b with 
                                                                            | Block.Image(v) -> Some(Fragment.Image(v, Map.empty))
                                                                            | _ -> None)
                                        | _ -> Seq.empty)

    let getImageViewFromImage image (view:string) = 
        match image with
            | Image (v, s) -> 
                    match view.ToLowerInvariant() with
                            "main" -> Some(v)
                            | _ -> s |> Map.tryFind view
            | _ -> None



    let getImageView field view fragmentMap = 
        get field fragmentMap 
            |> Option.bind (fun f -> match f with   
                                        | Image(_) -> 
                                            getImageViewFromImage f view
                                        | StructuredText(_) when view = "main" -> 
                                            getImage field fragmentMap 
                                                |> Option.bind (function Image(m, v) -> Some(m)
                                                                                        | _ -> None)
                                        | x -> None)

    let getAllImageViews field view fragmentMap =
        getAll field fragmentMap 
            |> Seq.collect (fun f -> match f with   
                                        | Fragment.Image(_) -> Seq.singleton (getImageViewFromImage f view)
                                        | Fragment.StructuredText(_) when view = "main" -> 
                                            getAllImages field fragmentMap 
                                                |> Seq.map (function Image(m, v) -> Some(m) 
                                                                                    | _ -> None)
                                        | _ -> Seq.empty)
            |> Seq.choose id

    let getStructuredText field fragmentMap = 
        get field fragmentMap 
            |> Option.bind (fun f -> match f with   
                                        | StructuredText(_) -> Some(f) 
                                        | _ -> None)

    let getText field fragmentMap =         
        let textFromBlock = function Block.Text(t) -> match t with
                                                            Text.Heading(t, _, _) 
                                                            | Text.ListItem(t, _, _) 
                                                            | Text.Paragraph(t, _) 
                                                            | Text.Preformatted(t, _) -> Some(t)
                                                  | _ -> None
        let buildBlockText (blocks: Block seq) = 
            blocks |> Seq.choose textFromBlock |> String.concat "\n"
        let filterNotEmpty = function "" -> None | t -> Some(t)
        let buildBlockTextOption b = filterNotEmpty (buildBlockText b)
            
        get field fragmentMap 
            |> Option.bind (fun f -> match f with   
                                        | StructuredText(blocks) -> 
                                            blocks |> buildBlockTextOption 
                                        | Number(f) -> Some(f.ToString())
                                        | Color(c) -> Some(c.hex)
                                        | Text(t) -> filterNotEmpty t
                                        | Date(d) -> Some(d.ToString()) // format date
                                        | _ -> None)
    let getColor field fragmentMap = 
        get field fragmentMap 
            |> Option.bind (fun f -> match f with   
                                        | Color(_) -> Some(f) 
                                        | _ -> None)
    
    let getNumber field fragmentMap = 
        get field fragmentMap 
            |> Option.bind (fun f -> match f with   
                                        | Number(_) -> Some(f)
                                        | _ -> None)


    let getDate field fragmentMap = 
        get field fragmentMap 
            |> Option.bind (fun f -> match f with   
                                        | Date(_) -> Some(f) 
                                        | _ -> None) 

    let getBoolean field fragmentMap = 
        get field fragmentMap 
            |> Option.bind (fun f -> match f with   
                                        | Text(t) -> match t.ToLowerInvariant() with
                                                            "yes" -> Some(true)
                                                            | "true" -> Some(true)
                                                            | _ -> None
                                        | _ -> None)
            |> Option.isSome

    let getGroup field fragmentMap = 
        get field fragmentMap 
            |> Option.bind (fun f -> match f with   
                                        | Group(_) -> Some(f) 
                                        | _ -> None)

