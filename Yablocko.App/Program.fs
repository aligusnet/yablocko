open System
open Newtonsoft.Json
open Yablocko

let prompt = """=========================
1. Connect to a server
2. Add a transaction
3. Display Blockchain
4. Exit
========================="""

type Context = { 
    Client : P2PClient
    Name : string
}

let connect (ctx : Context) =
    printfn "Please enter the server URL:"
    let url = Console.ReadLine ()
    ctx.Client.Connect (sprintf "%s/Blockchain" url)

let addTransaction (ctx : Context) =
    printfn "Please enter the receiver name:"
    let receiver = Console.ReadLine ()
    printfn "Please enter the amount"
    let amount = Console.ReadLine() |> int
    Yacoin.get ()
    |> Blockchain.createTransaction ctx.Name receiver amount
    |> Blockchain.processPendingTransactions ctx.Name
    |> Yacoin.set
    |> Yacoin.serialize
    |> ctx.Client.Broadcast

let displayBlockchain (_ : Context) =
    printfn "Blockchain"
    JsonConvert.SerializeObject(Yacoin.get (), Formatting.Indented)
    |> printfn "%s"
    
let onEvent (ctx : Context) = function
    | "1" -> connect ctx
    | "2" -> addTransaction ctx
    | "3" -> displayBlockchain ctx
    | _ -> ()


let rec eventLoop (ctx : Context) =
    printfn "Please select an action"
    let e = Console.ReadLine ()
    if e <> "4" then
        onEvent ctx e
        eventLoop ctx


[<EntryPoint>]
let main argv =
    let address = "127.0.0.1"
    let port = if Seq.length argv > 0 then Some (int argv.[0]) else None
    let name = if Seq.length argv > 1 then argv.[1] else "Unknown"
    let _server = Option.map (fun port -> new P2PServer(address, port)) port
                 |> Option.iter (fun server -> server.Start ())

    use client = new P2PClient()

    Yacoin.set (Blockchain.create 2 1) |> ignore

    let ctx = {Client = client; Name = name}

    printfn "Current user is %s" name
    printfn "%s" prompt
    
    eventLoop ctx

    0
