module tokenizer

let line_split(s:string) =
    s.Split(' ') |> Array.toList

    
let tokenize(s:string) =
    let mutable tokens : string list = []
    let lines = s.Split('\n')
    for line in lines do
        let t = line_split line
        tokens <- tokens @ t
    tokens



