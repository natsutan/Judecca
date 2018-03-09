module parser_input
open parser_net


type TENSOR_TYPE = { elem_type : OTPYE ; shape : int list } 

type  OINPUT_TYPE = TENSOR_TYPE

type OINPUT = { name : string ; itype : OINPUT_TYPE}

type OINPUT_LIST = OINPUT list

let dummy_input = { name = "dummy" ; itype = { elem_type = INT ; shape = [] }}

let init_inputs : OINPUT_LIST = 
    [dummy_input]

let parse_name(ts : TokenStream) : string =
    let t = ts.get()
    if t <> "name:" then
        raise(ParseError(t))
    ts.get() |> removeDQs

let parse_elem_type(ts : TokenStream) : OTPYE =
    let t = ts.get()
    if t <> "elem_type:" then
        raise(ParseError(t))
    ts.get() |> toOtype

let parse_shape(ts : TokenStream) : int list =
    let mutable il : int list = []
    let mutable t = ts.get()
    if t <> "shape" then
        raise(ParseError(t))
    ts.get() |> ignore // { のスキップ
    
    t <- ts.get()
    while t = "dim" do
        ts.get() |> ignore // '{'のスキップ
        t <- ts.get()
        if t <> "dim_value:" then
            raise(ParseError(t))
        let dv = ts.get() |> int
        il <- il @ [dv]
        ts.get() |> ignore // } のスキップ
        t <- ts.get()
    ts.get() |> ignore // } のスキップ
    il

let parse_type(ts : TokenStream) : OINPUT_TYPE =
    let t = ts.get()
    if t <> "type" then
        raise(ParseError(t))
    ts.get() |> ignore // '{' のskip
    let itype = ts.get()
    match itype with
    | "tensor_type" ->
        ts.get() |> ignore // '{' のskip
        let elem_type = parse_elem_type(ts)
        let shape = parse_shape(ts)
        ts.get() |> ignore // '}' のskip
        //ts.get() |> ignore // '}' のskip
        {elem_type = elem_type ; shape = shape }        

    | _ ->  raise(ParseError(itype))
 
let parse_input_core(ts : TokenStream) : OINPUT_LIST =
    let mutable oi = init_inputs

    while ts.is_last() = false do
        let name = parse_name(ts)
        let itype = parse_type(ts)
        oi <- oi @ [{name = name ; itype = itype}]
    oi

let parse_input (ts : string list) : OINPUT_LIST =
    let token_strem = TokenStream(ts)
    parse_input_core(token_strem
    )
