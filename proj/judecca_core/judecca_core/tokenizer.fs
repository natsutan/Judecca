module tokenizer

let line_split(s:string) =
    let mutable tokens = []
    //文字列を切り出すときに使う変数
    let mutable tmp = ""
    let mutable inStringSeq = false
    for c in s do
        let cs = c.ToString()
        match c with
        | ' ' | '\n' ->
            if inStringSeq then
                tmp <- String.concat "" [tmp;cs]
            else
                //切り出した文字列が空で無ければトークンを追加し、切り出した文字列を空にする。
                if tmp <> "" then
                    tokens <- List.append tokens [tmp]
                    tmp <- ""
        | '"' -> 
            inStringSeq <- not inStringSeq
            tmp <- String.concat "" [tmp;cs]
        | _ -> tmp <- String.concat "" [tmp;cs]

    if tmp <> "" then
        tokens <- List.append tokens [tmp]

    tokens
//    s.Split(' ') |> Array.toList

    
let tokenize(s:string) =
    let mutable tokens : string list = []
    let lines = s.Split('\n')
    for line in lines do
        let t = line_split line
        tokens <- tokens @ t
    tokens



