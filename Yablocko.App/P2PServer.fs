namespace Yablocko

open WebSocketSharp.Server

type P2PServer(ipAddress:string, port : int) =
    inherit WebSocketBehavior()
     
    let address = sprintf "ws://%s:%d" ipAddress port
    let wss = new WebSocketServer(address)
    do wss.AddWebSocketService<P2PServer> "/Blockchain"
    let mutable chainSynched = false

    new() = P2PServer("127.0.0.1", 7001)

    member __.Start () = 
        wss.Start ()
        printfn "Started server at %s" address

    override this.OnMessage e =
        if e.Data = "Hi Server" then
            printfn "%s" e.Data
            this.Send "Hi Client"
        else
            e.Data
            |> Yacoin.deserialize
            |> Yacoin.setIfBigger

            if not chainSynched then
                this.Send (Yacoin.get () |> Yacoin.serialize)
                chainSynched <- true
