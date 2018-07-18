module BlockchainTests

#nowarn "25"  // Ignore 'Incomplete pattern matches on this expression' warning.

open System
open Xunit
open FsUnit.Xunit

open Yablocko

/// Default level of mining difficulty using in tests
let difficulty = 2

[<Fact>]
let ``We should be able to create new Blockchain`` () =
    let bc =Blockchain.create difficulty
    (Blockchain.getLatestBlock bc).Index |> should equal 0
    Blockchain.isValid bc |> should equal true

[<Fact>]
let ``We should be able to add new blocks to a Blockchain`` () =
    let bc = Blockchain.create difficulty
          |> Blockchain.addBlock DateTime.Now "block1" 
          |> Blockchain.addBlock DateTime.Now "block2"

    let lastBlock = Blockchain.getLatestBlock bc
    lastBlock.Index |> should equal 2
    lastBlock.Data |> should equal "block2"
    Blockchain.isValid bc |> should equal true

[<Fact>]
let ``Blockschain contains block forged previous hash should be invalid`` () =
    let bc = Blockchain.create difficulty
          |> Blockchain.addBlock DateTime.Now "block1" 
          |> Blockchain.addBlock DateTime.Now "block2"
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
    let bc = Blockchain.create difficulty
          |> Blockchain.addBlock DateTime.Now "block1" 
          |> Blockchain.addBlock DateTime.Now "block2"
    let [block2; block1; block0] = bc.Chain

    let forgedDataChain = {bc with Blockchain.Chain = [block2; {block1 with Data = "fake"}; block0]}
    Blockchain.isValid forgedDataChain |> should equal false

    let forgedTimeStampChain = {bc with Blockchain.Chain = [block2; {block1 with TimeStamp = DateTime.Parse("2010-01-01")}; block0]}
    Blockchain.isValid forgedTimeStampChain |> should equal false

    // we still can restore the blockchain
    let originalChain = {bc with Blockchain.Chain = [block2; block1; block0]}
    Blockchain.isValid originalChain |> should equal true
