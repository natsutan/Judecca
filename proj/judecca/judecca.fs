open System.IO
open tokenizer
open parser_net


let network = "C:\\home\\myproj\\Judecca\\testdata\\squeezenet_onnx.net.txt"

[<EntryPoint>]
let main argv = 
    let s = File.ReadAllText(network)
    let tokens = tokenize s 
    let g =  parse_net tokens

    for n in g do
        printfn "%A" n


    0 // 整数の終了コードを返します
