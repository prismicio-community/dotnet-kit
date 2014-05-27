namespace prismic
open System.Web
open FSharp.Data
open FSharp.Data.JsonExtensions

module internal Shortcuts =

    let inline (<?-) a d = match a with | Some(v) -> v | None -> d // getOrElse (defaultArg)
    let inline (<?--) a (d:'d Lazy) = match a with | Some(v) -> v | None -> d.Force() // getOrElse lazy

    // helpers for json values
    let inline (>?) (x:JsonValue) propertyName = x.TryGetProperty propertyName
    let asString (jsonvalue:JsonValue) = jsonvalue.AsString()
    let asDateTime (jsonvalue:JsonValue) = jsonvalue.AsDateTime()
    let asBoolean (jsonvalue:JsonValue) = jsonvalue.AsBoolean()
    let asInteger (jsonvalue:JsonValue) = jsonvalue.AsInteger()
    let asSomethingOption f (jsonvalue:JsonValue option) = match jsonvalue with | Some(JsonValue.Null) -> None | Some(j) -> Some(f(j)) | None -> None
    let asDateTimeOption = asSomethingOption asDateTime
    let asBooleanOption = asSomethingOption asBoolean
    let asStringOption = asSomethingOption asString
    let asIntegerOption = asSomethingOption asInteger
    let asArrayOrEmpty (x:JsonValue) propertyName = match x.TryGetProperty propertyName with Some(a) -> a.AsArray() | None -> [||]

    let asMapFromOptionProperties (tovalue:(JsonValue -> 'a option)) (jsonvalue:JsonValue) 
        = jsonvalue.Properties 
            |> Array.fold (fun (m:Map<string,'a>) (k,v) -> 
                    tovalue(v) |> (Option.fold (fun (s:Map<string,'a>) t -> s.Add(k, t))) m) 
                    Map.empty 

    let asMapFromProperties (tovalue:(JsonValue -> 'a)) (jsonvalue:JsonValue) 
        = jsonvalue.Properties |> Array.fold (fun (m:Map<string,'a>) (k,v) -> m.Add(k, tovalue(v))) Map.empty 
    let asStringMapFromProperties = asMapFromProperties asString


    let (|JsonArray|_|) (x:JsonValue) = match x with JsonValue.Array elements -> Some(elements) | _ -> None
        
    // active pattern for parsing regex and capture matches 
    let (|ParseRegex|_|) regex str =
                               let m = System.Text.RegularExpressions.Regex(regex).Match(str)
                               if m.Success
                               then Some (List.tail [ for x in m.Groups -> x.Value ])
                               else None

    let tryParseHexColor = function
     | ParseRegex "^#([0-9a-fA-F]{2})([0-9a-fA-F]{2})([0-9a-fA-F]{2})$" d -> Some(d)
     | _ -> None

    let tryParseIndexedKey = function
         | ParseRegex "^([^\[]+)(\[\d+\])?$" d -> Some(d)
         | _ -> None

    
    // shortcut for head option
    let seqheadoption s = s |> Seq.tryPick Some

    // html encode
    let htmlEncode = HttpUtility.HtmlEncode

    /// Splits this list into a prefix/suffix pair according to a predicate
    /// returns a pair consisting of the longest prefix of this list whose elements all satisfy pred, and the rest of this list.
    let span (pred:'a -> bool) (a:'a list) = 
            let rec loop acc l = match l with
                                    head :: tail -> if pred(head) then loop (head :: acc) tail 
                                                                  else (acc, l)
                                    | [] -> (acc, [])
            let (acc, rest) = loop [] a
            (acc |> List.rev, rest)


    // a simple list for better cons 
    type 'a hlist = 'a list -> 'a list  
    let empty : 'a hlist = let id xs = xs in id
    let append xs ys = fun tail -> xs (ys tail)
    let singleton x = fun tail -> x :: tail
    let cons x xs = append (singleton x) xs
    let snoc xs x = append xs (singleton x)
    let toList : 'a hlist -> 'a list = fun xs -> xs []

    let (@=) = cons
    let (=@) = snoc

