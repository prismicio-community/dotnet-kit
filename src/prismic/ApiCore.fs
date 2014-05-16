namespace prismic
open System
open System.Collections.Specialized
open System.IO
open System.Net
open System.Reflection
open System.Web

module ApiCore =

    let inline reraisePreserveStackTrace (e:Exception) =
        let remoteStackTraceString = typeof<exn>.GetField("_remoteStackTraceString", BindingFlags.Instance ||| BindingFlags.NonPublic);
        remoteStackTraceString.SetValue(e, e.StackTrace + Environment.NewLine);
        raise e

    type httpResponse = { statusCode:HttpStatusCode ; statusText:string ; body:string }

    let fetch(request: WebRequest) = async {
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

        try
            use! response = request.AsyncGetResponse()
            let! r = read response

            let httpResponse = (response :?> HttpWebResponse) in
            return { statusCode = httpResponse.StatusCode ; statusText = httpResponse.StatusDescription ;  body = r }
        with 
            | :? WebException as ex -> 
                let response = ex.Response :?> HttpWebResponse
                match response with 
                    | null -> return reraisePreserveStackTrace ex
                    | _ ->  let! r = read response
                            return { statusCode = response.StatusCode ; statusText = response.StatusDescription ; body = r }
            | e -> return reraisePreserveStackTrace e
    }

    let buildUrl baseurl (values:Map<string, string seq>) = 
        let b = System.Uri(baseurl, UriKind.Absolute)
        (*
        TODO : check UTF8 encoding 
        use Uri.EscapeDataString, HttpUtility.UrlEncode
        *)
        let httpValueCollection = HttpUtility.ParseQueryString(b.Query)
        let nameValueCollection = NameValueCollection()
        values |> Map.iter (fun k s -> s |> Seq.iter (fun v -> nameValueCollection.Add(k, v)))
        httpValueCollection.Add(nameValueCollection)
        let queryString = httpValueCollection.ToString()
        let ub = UriBuilder(b)
        ub.Query <- queryString
        ub.Uri

