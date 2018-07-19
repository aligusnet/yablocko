module BlockTests

open System
open Xunit
open FsUnit.Xunit

open Yablocko

/// Default level of mining difficulty using in tests
let difficulty = 2

let testTranList1 = [Transaction.create "A" "B" 11; Transaction.create "B" "C" 9 ]
let testTranList2 = [Transaction.create "C" "A" 7 ]


[<Fact>]
let ``We should be able to create initial block`` () =
    let timeStamp = DateTime.Now
    let block = Block.createInitial difficulty timeStamp []
    block.Transactions |> should equal ([] : Transaction.T list)
    block.Index |> should equal 0
    block.PreviousHash |> should equal ""
    block.TimeStamp |> should equal timeStamp
    block.Hash |> should startWith (String.replicate difficulty "0")
    Block.isValid "" block |> should equal true


[<Fact>]
let ``We should be able to create blocks`` () =
    let timeStamp = DateTime.Now
    let initialBlock = Block.createInitial difficulty (DateTime.Parse "2011-11-11") []
    
    let timeStamp1 = timeStamp.Subtract (TimeSpan.FromHours 2.0)
    let block1 = Block.create difficulty timeStamp1 testTranList1 initialBlock
    block1.Transactions |> should equal testTranList1
    block1.Index |> should equal 1
    block1.PreviousHash |> should equal initialBlock.Hash
    block1.TimeStamp |> should equal timeStamp1
    block1.Hash |> should startWith (String.replicate difficulty "0")
    Block.isValid initialBlock.Hash block1 |> should equal true

    let timeStamp2 = timeStamp.Subtract (TimeSpan.FromHours 1.0)
    let block2 = Block.create difficulty timeStamp2 testTranList2 block1
    block2.Transactions |> should equal testTranList2
    block2.Index |> should equal 2
    block2.PreviousHash |> should equal block1.Hash
    block2.TimeStamp |> should equal timeStamp2
    block2.Hash |> should startWith (String.replicate difficulty "0")
    Block.isValid block1.Hash block2 |> should equal true

[<Fact>]
let ``Blocks with forged previous hash should be invalid`` () =
    let block = Block.createInitial difficulty DateTime.Now []
             |> Block.create difficulty DateTime.Now testTranList1
    let originalPreviousHash = block.PreviousHash
    let forgedHash = originalPreviousHash + "_FAKE"
    Block.isValid forgedHash block |> should equal false
    Block.isValid originalPreviousHash {block with Block.PreviousHash = forgedHash} |> should equal false

[<Fact>]
let ``Forged blocks should be invalid`` () =
    let block = Block.createInitial difficulty DateTime.Now []
             |> Block.create difficulty DateTime.Now testTranList2
    let originalPreviousHash = block.PreviousHash
    Block.isValid originalPreviousHash {block with Block.Transactions = testTranList1} |> should equal false
    Block.isValid originalPreviousHash {block with Block.TimeStamp = DateTime.Parse "2000-01-01"} |> should equal false
