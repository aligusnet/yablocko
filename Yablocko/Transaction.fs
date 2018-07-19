namespace Yablocko

module Transaction =
    type T = {
        FromAddress : string
        ToAddress : string
        Amount : int
    }

    let create fromAddress toAddress amount = {
        FromAddress = fromAddress
        ToAddress = toAddress
        Amount = amount
    }
