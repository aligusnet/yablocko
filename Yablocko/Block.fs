namespace Yablocko

open System
open System.Security.Cryptography
open System.Text

module Block =
    type T = {
        /// Index of the block in the chain
        Index : int
        /// Creation date and time of the block
        TimeStamp : DateTime
        /// Hash value of the previous block from the chain. Empty for the initial block.
        PreviousHash : string
        /// Pre-computed hash value used to validate the block
        Hash : string
        /// Block's payload
        Data : string
        /// Value that brings the required number of leading zeroes to the hash value
        Nonce : int
    }

    let private calculateHash (timeStamp : DateTime) previousHash data nonce =
        let sha256 = SHA256.Create()
        sprintf "%A-%s-%s-%d" timeStamp previousHash data nonce
            |> Encoding.ASCII.GetBytes
            |> sha256.ComputeHash
            |> Convert.ToBase64String

    /// Mine function returns a hash value with number of leading zeroes equals value of difficulty
    /// and value of nonce that required to get such number of leading zeroes in the hash
    let private mine (timeStamp : DateTime) previousHash data difficulty =
        let leading = String.replicate difficulty "0"
        let calc = calculateHash timeStamp previousHash data

        let rec helper nonce =
            let hash = calc nonce
            if hash.StartsWith leading then (hash, nonce)
            else helper (nonce + 1)

        helper 0

    
    /// **Description**
    /// Create new block
    /// **Parameters**
    ///   * `difficulty` - a number of leading zeroes in hash value.
    ///   * `timeStamp` - timestamp of the block creation
    ///   * `data` - payload
    ///   * `previous` - previous block in the chain
    ///
    /// **Output Type**
    ///   * `T`
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

    
    /// **Description**
    /// Create initial block in the chain.
    /// **Parameters**
    ///   * `difficulty` - a number of leading zeroes in hash value.
    ///   * `timeStamp` - timestamp of the block creation
    ///   * `data` - payload
    /// 
    /// **Output Type**
    ///   * `T`
    ///
    /// **Exceptions**
    /// No exceptions
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

    
    /// **Description**
    /// Tests if the current block valid;.
    /// **Parameters**
    ///   * `previousHash` - hash of the previous block in the chain
    ///   * `block` - block to be tested
    ///
    /// **Output Type**
    ///   * `bool` - true if block is valid
    ///
    /// **Exceptions**
    /// No exceptions
    let isValid previousHash (block : T) =
        block.PreviousHash = previousHash && block.Hash = calculateHash block.TimeStamp block.PreviousHash block.Data block.Nonce
