namespace Yablocko

open System
open System.Security.Cryptography
open System.Text

module Block =
    type T = {
        Index : int
        TimeStamp : DateTime
        PreviousHash : string
        Hash : string
        Data : string
    }

    let private calculateHash (timeStamp : DateTime) previousHash data =
        let sha256 = SHA256.Create()
        Encoding.ASCII.GetBytes( sprintf "%A-%s-%s" timeStamp previousHash data )
            |> sha256.ComputeHash
            |> Convert.ToBase64String

    let create (timeStamp : DateTime) data (previous : T) = {
        Index = previous.Index + 1
        TimeStamp = timeStamp
        PreviousHash = previous.Hash
        Hash = calculateHash timeStamp previous.Hash data
        Data = data
    }

    let createInitial (timeStamp : DateTime) data =
        let previousHash = "" in {
            Index = 0
            TimeStamp = timeStamp
            PreviousHash = previousHash
            Hash = calculateHash timeStamp previousHash data
            Data = data
        }

    let isValid previousHash (block : T) =
        block.PreviousHash = previousHash && block.Hash = calculateHash block.TimeStamp block.PreviousHash block.Data
