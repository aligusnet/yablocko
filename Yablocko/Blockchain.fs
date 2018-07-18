namespace Yablocko

open System

module Blockchain =
    type T = {
        Chain : Block.T list
    }

    let create = {
        Chain = [ Block.createInitial DateTime.Now "{}" ]
    }

    let getLatestBlock { Chain = chain } =
        Seq.head chain

    let addBlock (timeStamp : DateTime) (data : string) (blockchain : T) = {
        Chain = (Block.create timeStamp data (getLatestBlock blockchain)) :: blockchain.Chain
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
