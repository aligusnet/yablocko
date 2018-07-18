module Tests


open Xunit
open FsUnit.Xunit

[<Fact>]
let ``1 + 1 equals 2, sometimes`` () =
    1 + 1 |> should equal 2


[<Fact>]
let ``1 + 1 equals 10, sometimes`` () =
    0b01 + 0b01 |> should equal 0b10


[<Fact>]
let ``1 + 1 equals 11, sometimes`` () =
    "1" + "1" |> should equal "11"


[<Fact>]
let ``Say Hello to Tiffany Aching``() =
    Yablocko.Say.hello "Tiffany Aching" |> should equal "Hello from Blockchain, Tiffany Aching!"
