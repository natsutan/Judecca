open System.IO
open tokenizer
open parser_net
open parser_input

let network = "C:\\home\\myproj\\Judecca\\testdata\\squeezenet_onnx.net.txt"
let input = "C:\\home\\myproj\\Judecca\\testdata\\squeezenet_onnx.input.txt"

[<EntryPoint>]
let main argv = 
    let s = File.ReadAllText(network)
    let tokens = tokenize s 
    let g =  parse_net tokens

    for n in g do
        printfn "%A" n

    let s2 = File.ReadAllText(input)
    let tokens2 = tokenize s2
    let il = parse_input tokens2

    for n in il do
        printfn "%A" n


    0 // 整数の終了コードを返します
