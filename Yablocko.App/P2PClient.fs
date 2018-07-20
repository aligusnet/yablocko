namespace Yablocko

open System
open System.Collections.Generic
open WebSocketSharp

type P2PClient() =
    let wsDict = new Dictionary<string, WebSocket>()

    member __.Connect url = 
        if not (wsDict.ContainsKey url) then
            let ws = new WebSocket(url)
            ws.OnMessage.Add (fun e -> 
                if e.Data = "Hi Client" then
                    printfn "%s" e.Data
                else
                    e.Data
                    |> Yacoin.deserialize
                    |> Yacoin.setIfBigger
                )
            ws.Connect ()
            ws.Send "Hi Server"
            wsDict.Add (url, ws)

    member __.Send url (data : string) =
        if wsDict.ContainsKey url then
            wsDict.[url].Send data

    member __.Broadcast (data : string) =
        wsDict.Values 
        |> Seq.iter (fun ws -> ws.Send data)

    member __.Close () =
        wsDict.Values 
        |> Seq.iter (fun ws -> ws.Close ())

    interface IDisposable with
        member this.Dispose() = this.Close ()
