namespace Yablocko

open System

module Blockchain =
    type T = {
        Chain : Block.T list
        Difficulty : int
    }

    let create difficulty = {
        Chain = [ Block.createInitial 0 DateTime.Now "{}" ]
        Difficulty = difficulty
    }

    let getLatestBlock { Chain = chain } =
        Seq.head chain

    let addBlock (timeStamp : DateTime) (data : string) (blockchain : T) = {
        blockchain with
            Chain = (Block.create blockchain.Difficulty timeStamp data (getLatestBlock blockchain)) :: blockchain.Chain
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
