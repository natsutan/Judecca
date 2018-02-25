// F# の詳細については、http://fsharp.org を参照してください
// 詳細については、'F# チュートリアル' プロジェクトを参照してください。
open NUnit.Framework
open tokenizer

type token_test_data(indata:string, expected:string list) =
    let _indata = indata
    let _expected = expected
    member this.indata = _indata
    member this.expected = _expected


let test1 = token_test_data("A 2 3 4" , ["A";"2";"3";"4"])
let test2 = token_test_data("1 2" , ["1";"2"])

let token_test_sorces = [test1; test2]

[<TestCaseSource("token_test_sorces")>]
let tokenizer_test0(pair:token_test_data) = 
    let result = tokenizer.tokenize pair.indata
    Assert.AreEqual(result,  pair.expected)

[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    0 // 整数の終了コードを返します
