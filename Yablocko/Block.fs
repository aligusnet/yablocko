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
        Nonce : int
    }

    let private calculateHash (timeStamp : DateTime) previousHash data nonce =
        let sha256 = SHA256.Create()
        sprintf "%A-%s-%s-%d" timeStamp previousHash data nonce
            |> Encoding.ASCII.GetBytes
            |> sha256.ComputeHash
            |> Convert.ToBase64String

    let private mine (timeStamp : DateTime) previousHash data difficulty =
        let leading = String.replicate difficulty "0"
        let calc = calculateHash timeStamp previousHash data

        let rec helper nonce =
            let hash = calc nonce
            if hash.StartsWith leading then (hash, nonce)
            else helper (nonce + 1)

        helper 0

    let create difficulty (timeStamp : DateTime) data (previous : T) = 
        let hash, nonce = mine timeStamp previous.Hash data difficulty
        {
            Index = previous.Index + 1
            TimeStamp = timeStamp
            PreviousHash = previous.Hash
            Hash = hash
            Data = data
            Nonce = nonce
        }

    let createInitial difficulty (timeStamp : DateTime) data =
        let previousHash = ""
        let hash, nonce = mine timeStamp previousHash data difficulty
        {
            Index = 0
            TimeStamp = timeStamp
            PreviousHash = previousHash
            Hash = hash
            Data = data
            Nonce = nonce
        }

    let isValid previousHash (block : T) =
        block.PreviousHash = previousHash && block.Hash = calculateHash block.TimeStamp block.PreviousHash block.Data block.Nonce
