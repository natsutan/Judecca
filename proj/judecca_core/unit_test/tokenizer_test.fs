// F# の詳細については、http://fsharp.org を参照してください
// 詳細については、'F# チュートリアル' プロジェクトを参照してください。
open NUnit.Framework
open tokenizer

[<Test>]
let tokenizer_test0() = 
    let s = "1 2 3　4"
    let result = tokenizer.tokenize s
    Assert.AreEqual(result, ["1";"2";"3";"4"])

[<EntryPoint>]
let main argv = 
    printfn "%A" argv
    0 // 整数の終了コードを返します
