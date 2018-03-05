module tokenizer_Test
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
let test2 = token_test_data("1 2 " , ["1";"2"])
let test3 = token_test_data("input: \"1\"\n" , ["input:";"\"1\""])
let test4 = token_test_data("\"1 2 \"\n" , ["\"1 2 \""])
let test5 = token_test_data("attribute {\n  name: \"kernel_shape\"\n  ints: 3\n ints: 3\n type: INTS\n  }" , ["attribute"; "{"; "name:"; "\"kernel_shape\""; "ints:" ; "3" ; "ints:"; "3" ; "type:"; "INTS"; "}"])
let test6 = token_test_data("1 2\r\n5" , ["1";"2";"5"])

let token_test_sorces = [test1; test2; test3; test4; test5; test6]

[<Category("tokenizer")>]
[<TestCaseSource("token_test_sorces")>]
let tokenizer_test0(pair:token_test_data) = 
    let result = tokenizer.tokenize pair.indata
    Assert.AreEqual(result,  pair.expected)


 
