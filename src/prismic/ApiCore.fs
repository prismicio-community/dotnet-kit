namespace prismic
open System
open System.Collections.Specialized
open System.IO
open System.Linq
open System.Net
open System.Reflection
open System.Web

module internal ApiCore =

    let inline reraisePreserveStackTrace (e:Exception) =
        let remoteStackTraceString = typeof<exn>.GetField("_remoteStackTraceString", BindingFlags.Instance ||| BindingFlags.NonPublic);
        remoteStackTraceString.SetValue(e, e.StackTrace + Environment.NewLine);
        raise e

    type httpResponse = { statusCode:HttpStatusCode ; statusText:string ; body:string ; headers:Map<string, string[]> }

    let fetch (request: WebRequest) (logger:string->string->unit) = async {
        let read (response: WebResponse) = async {
            use stream = response.GetResponseStream()
            let mem = new System.IO.MemoryStream()
            let buffer = Array.zeroCreate 4096

            let rec download() = async { 
                let! count = stream.AsyncRead(buffer, 0, buffer.Length)
                do! mem.AsyncWrite(buffer, 0, count)
                if count > 0 then return! download() }

            do! download()
            mem.Seek(0L, SeekOrigin.Begin) |> ignore
            use reader = new StreamReader(mem)
            let responseRead = reader.ReadToEnd()
            return responseRead
        }

        let mapHeaders (headers:WebHeaderCollection) = 
            seq { for i in 0 .. headers.Count-1 do 
                    let key = headers.GetKey(i)
                    yield (key, headers.GetValues(i)) } |> Map.ofSeq

        try
            use! response = request.AsyncGetResponse()
            let! r = read response

            let httpResponse = (response :?> HttpWebResponse) in
            let headers = mapHeaders httpResponse.Headers
            return { statusCode = httpResponse.StatusCode ; statusText = httpResponse.StatusDescription ;  body = r ; headers = headers }
        with 
            | :? WebException as ex -> // there was an exception but we have a response
                let response = ex.Response :?> HttpWebResponse
                match response with 
                    | null -> return reraisePreserveStackTrace ex
                    | _ ->  let! r = read response
                            let headers = mapHeaders response.Headers
                            return { statusCode = response.StatusCode ; statusText = response.StatusDescription ; body = r ; headers = headers }
            | e -> return reraisePreserveStackTrace e // no response, throw (copy stack since we are in an async computation)
    }

    let buildUrl baseurl (values:Map<string, string seq>) = 
        let b = System.Uri(baseurl, UriKind.Absolute)
        (*
        TODO : check UTF8 encoding 
        use Uri.EscapeDataString, HttpUtility.UrlEncode
        *)
        let httpValueCollection = HttpUtility.ParseQueryString(b.Query) // gets a collection with the initial querystring args
        let nameValueCollection = NameValueCollection()
        values |> Map.iter (fun k s -> s |> Seq.iter (fun v -> nameValueCollection.Add(k, v))) // adds the values to a new collection
        httpValueCollection.Add(nameValueCollection) // merge the new collection with initial collection 
        let queryString = httpValueCollection.ToString()
        let ub = UriBuilder(b) // build the uri
        ub.Query <- queryString
        ub.Uri

