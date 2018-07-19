module TransactionTests

open Xunit
open FsUnit.Xunit

open Yablocko


[<Theory>]
[<InlineData("A", "B", 11)>]
let ``We should be able to create new transactions`` (fromAddress, toAddress, amount) =
    let tran = Transaction.create fromAddress toAddress amount
    tran.FromAddress |> should equal fromAddress
    tran.ToAddress |> should equal toAddress
    tran.Amount |> should equal amount
