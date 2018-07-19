namespace Yablocko

open System

module Blockchain =
    type T = {
        Chain : Block.T list
        Difficulty : int
        PendingTransactions : Transaction.T list
    }

    let reward = 1

    let create difficulty = {
        Chain = [ Block.createInitial 0 DateTime.Now [] ]
        Difficulty = difficulty
        PendingTransactions = []
    }

    let getLatestBlock { Chain = chain } =
        Seq.head chain

    let addBlock (timeStamp : DateTime) transactions (blockchain : T) = {
        blockchain with
            Chain = (Block.create blockchain.Difficulty timeStamp transactions (getLatestBlock blockchain)) :: blockchain.Chain
    }

    let rec private isChainValid (chain : Block.T list) =
        match chain with
        | [] -> true
        | [node] -> (Block.isValid "" node)
        | node :: remains ->
            if Block.isValid (Seq.head remains).Hash node then
                isChainValid remains
            else
                false

    let isValid {Chain = chain} = isChainValid chain

    let createTransaction fromAddress toAddress amount (blockchain : T) =
        {
        blockchain with
            PendingTransactions = (Transaction.create fromAddress toAddress amount) :: blockchain.PendingTransactions
    }

    let processPendingTransactions miner (blockchain : T) = {
        blockchain with
            Chain = (Block.create blockchain.Difficulty DateTime.Now blockchain.PendingTransactions (getLatestBlock blockchain)) :: blockchain.Chain
            PendingTransactions = [Transaction.create "" miner reward]
    }
