module BlockchainTests

#nowarn "25"  // Ignore 'Incomplete pattern matches on this expression' warning.

open System
open Xunit
open FsUnit.Xunit

open Yablocko

/// Default level of mining difficulty using in tests
let difficulty = 2
let reward = 1

let testTranList1 = [Transaction.create "A" "B" 11; Transaction.create "B" "C" 9 ]
let testTranList2 = [Transaction.create "C" "A" 7 ]

[<Fact>]
let ``We should be able to create new Blockchain`` () =
    let bc =Blockchain.create difficulty reward
    (Blockchain.getLatestBlock bc).Index |> should equal 0
    Blockchain.isValid bc |> should equal true
    bc.Reward |> should equal reward
    Seq.length bc.PendingTransactions |> should equal 0

[<Fact>]
let ``We should be able to add new blocks to a Blockchain`` () =
    let bc = Blockchain.create difficulty reward
          |> Blockchain.addBlock DateTime.Now testTranList1
          |> Blockchain.addBlock DateTime.Now testTranList2

    let lastBlock = Blockchain.getLatestBlock bc
    lastBlock.Index |> should equal 2
    lastBlock.Transactions |> should equal testTranList2
    Blockchain.isValid bc |> should equal true

[<Fact>]
let ``Blockschain contains block forged previous hash should be invalid`` () =
    let bc = Blockchain.create difficulty reward
          |> Blockchain.addBlock DateTime.Now testTranList1
          |> Blockchain.addBlock DateTime.Now testTranList2
    let [block2; block1; block0] = bc.Chain
    let originalPreviousHash = block1.PreviousHash
    let forgedHash = originalPreviousHash + "_FAKE"
    let forgedBlock1 = {block1 with PreviousHash = forgedHash}
    let forgedChain = {bc with Blockchain.Chain = [block2; forgedBlock1; block0]}
    Blockchain.isValid forgedChain |> should equal false

    // we still can restore the blockchain
    let originalChain = {bc with Blockchain.Chain = [block2; block1; block0]}
    Blockchain.isValid originalChain |> should equal true

[<Fact>]
let ``Blockchain with forged blocks should be invalid`` () =
    let bc = Blockchain.create difficulty reward
          |> Blockchain.addBlock DateTime.Now testTranList2
          |> Blockchain.addBlock DateTime.Now testTranList1
    let [block2; block1; block0] = bc.Chain

    let forgedTransChain = {bc with Blockchain.Chain = [block2; {block1 with Transactions = testTranList1}; block0]}
    Blockchain.isValid forgedTransChain |> should equal false

    let forgedTimeStampChain = {bc with Blockchain.Chain = [block2; {block1 with TimeStamp = DateTime.Parse("2010-01-01")}; block0]}
    Blockchain.isValid forgedTimeStampChain |> should equal false

    // we still can restore the blockchain
    let originalChain = {bc with Blockchain.Chain = [block2; block1; block0]}
    Blockchain.isValid originalChain |> should equal true


[<Theory>]
[<InlineData("A", "B", 17)>]
let ``We should be able to create transactions`` (fromAddress, toAddress, amount) =
    let bc = Blockchain.create difficulty reward
          |> Blockchain.createTransaction fromAddress toAddress amount
    let tran = Seq.head bc.PendingTransactions
    tran.FromAddress |> should equal fromAddress
    tran.ToAddress |> should equal toAddress
    tran.Amount |> should equal amount

[<Fact>]
let ``We should be able to process pending transactions`` () =
    let miner = "Mr. Miner"
    let bc = Blockchain.create difficulty reward
          |> Blockchain.createTransaction "A" "B" 7
          |> Blockchain.createTransaction "C" "B" 11

    let bc1 = Blockchain.processPendingTransactions miner bc
    (Blockchain.getLatestBlock bc1).Transactions |> should equal bc.PendingTransactions
    let rewardTransaction = Seq.head bc1.PendingTransactions
    rewardTransaction.ToAddress |> should equal miner


[<Fact>]
let ``Blockchain should calculate correct balance`` () =
    let bc = Blockchain.create difficulty reward
          |> Blockchain.createTransaction "" "A" 10
          |> Blockchain.createTransaction "" "B" 10
          |> Blockchain.createTransaction "" "C" 10
          |> Blockchain.processPendingTransactions "A"  // A = 10, B = 10, C = 10
          |> Blockchain.createTransaction "A" "B" 3
          |> Blockchain.createTransaction "C" "B" 5
          |> Blockchain.processPendingTransactions "B"  // A = 8, B = 18, C = 5
    
    Blockchain.getBalance "A" bc |> should equal 8
    Blockchain.getBalance "B" bc |> should equal 18
    Blockchain.getBalance "C" bc |> should equal 5

[<Fact>]
let ``Blockchain should calculate correct pending balance`` () =
    let bc = Blockchain.create difficulty reward
          |> Blockchain.createTransaction "A" "B" 10
          |> Blockchain.createTransaction "A" "C" 5
          |> Blockchain.createTransaction "C" "A" 7
          
    
    Blockchain.getPendingBalance "A" bc |> should equal -8
    Blockchain.getPendingBalance "B" bc |> should equal 10
    Blockchain.getPendingBalance "C" bc |> should equal -2

    let bc1 = Blockchain.processPendingTransactions "A" bc
    Blockchain.getPendingBalance "A" bc1 |> should equal 1
    Blockchain.getPendingBalance "B" bc1 |> should equal 0
    Blockchain.getPendingBalance "C" bc1 |> should equal 0
