namespace Yablocko

open Newtonsoft.Json

module Yacoin =
    open Blockchain
    let mutable coin = create 2 1

    let get () = coin

    let set c = coin <- c; coin

    let mergeTransactionsAndSet c =
        set {c with PendingTransactions = List.append c.PendingTransactions coin.PendingTransactions}
        |> ignore

    let setIfBigger c =
        if Blockchain.isValid c && Seq.length c.Chain > Seq.length coin.Chain then
            mergeTransactionsAndSet c

    let serialize (blockchain : T) = JsonConvert.SerializeObject blockchain

    let deserialize = JsonConvert.DeserializeObject<T>
