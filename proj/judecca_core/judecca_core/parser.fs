module parser

open System.Runtime.InteropServices
open System.Security.Authentication.ExtendedProtection

exception ParseError of string

//文字列を自作ストリームとして扱うクラス
type TokenStream(buf : string list) =
    class
        let _buf : string list = buf
        let mutable p : int = 0

        member this.get() =
            if this.is_last() then
                ""
            else
                let t = _buf.[p]
                p <- p + 1
                t
        
        member this.back() = 
            match p with
            | 0 -> p <- 0
            | _ -> p <- p - 1

        member this.is_last() : bool =
            List.length buf = p
    end


//onnxのOP_TYPE
type OPTYPE = 
    | Abs
    | Add
    | And
    | ArgMax
    | ArgMin
    | AveragePool
    | BatchNormalization
    | Cast
    | Ceil
    | Clip
    | Concat
    | Constant
    | Conv
    | ConvTranspose
    | DepthToSpace
    | Div
    | Dropout
    | Elu
    | Equal
    | Exp
    | Flatten
    | Floor
    | GRU
    | Gather
    | Gemm
    | GlobalAveragePool
    | GlobalLpPool
    | GlobalMaxPool
    | Greater
    | HardSigmoid
    | Hardmax
    | InstanceNormalization
    | LRN
    | LSTM
    | LeakyRelu
    | Less
    | Log
    | LogSoftmax
    | LpNormalization
    | LpPool
    | MatMul
    | Max
    | MaxPool
    | MaxRoiPool
    | Mean
    | Min
    | Mul
    | Neg
    | Not
    | Or
    | PRelu
    | Pad
    | Pow
    | RNN
    | RandomNormal
    | RandomNormalLike
    | RandomUniform
    | RandomUniformLike
    | Reciprocal
    | ReduceL1
    | ReduceL2
    | ReduceLogSum
    | ReduceLogSumExp
    | ReduceMax
    | ReduceMean
    | ReduceMin
    | ReduceProd
    | ReduceSum
    | ReduceSumSquare
    | Relu
    | Reshape
    | Selu
    | Shape
    | Sigmoid
    | Size
    | Slice
    | Softmax
    | Softplus
    | Softsign
    | SpaceToDepth
    | Split
    | Sqrt
    | Squeeze
    | Sub
    | Sum
    | Tanh
    | Tile
    | TopK
    | Transpose
    | Unsqueeze
    | Xor
    | DUMMY //グラフの最初に入れるダミーオペレータ

//onnxの変数の型
type OTPYE = 
    | INT
    | INTS
 
type ATTRIBUTE =
    | KERNEL_SHAPE of int list
    | STRIDES of int list
    | PADS of int list
    | DILATIONS of int list
    | GROUP of int
    | BROADCAST of int
    | AIXS of int

//onnxのノード
type ONODE = {inputs : int list ; outputs : int list ; optype : OPTYPE ; doc_string : string ; attributes : ATTRIBUTE list}
let init_onode : ONODE = {inputs = []; outputs = []; optype = DUMMY; doc_string = ""; attributes = []}

//graphの操作
type OGRAPH = ONODE list

let init_graph() : OGRAPH =
    let dummy : ONODE = {inputs=[]; outputs=[]; optype=DUMMY; doc_string=""; attributes= []}
    [dummy]

let graph_add_new_node (g : OGRAPH, node : ONODE) : OGRAPH =
    match node.optype with
    | DUMMY -> g
    | _ -> g @ [node]

//nodeの操作
let node_add_inputs(node:ONODE, input : int) : ONODE =
    {inputs=node.inputs @ [input] ; 
      outputs=node.outputs; optype=node.optype; doc_string=node.doc_string; attributes=node.attributes }

let node_add_outputs(node:ONODE, output : int) : ONODE =
    {inputs=node.inputs; 
      outputs=node.outputs @ [output]; 
      optype=node.optype; doc_string=node.doc_string; attributes=node.attributes }

let node_set_optype(node:ONODE, op : OPTYPE) : ONODE =
    {inputs=node.inputs; outputs=node.outputs ; 
      optype=op; 
      doc_string=node.doc_string; attributes=node.attributes }

let node_set_docstr(node:ONODE, doc_str : string) : ONODE =
    {inputs=node.inputs; outputs=node.outputs ; optype=node.optype; 
      doc_string=doc_str;
      attributes=node.attributes }

let node_add_attributes(node:ONODE, attr : ATTRIBUTE) : ONODE =
    {inputs=node.inputs; outputs=node.outputs ; optype=node.optype; doc_string=node.doc_string;
      attributes=node.attributes @ [attr] }
 
//文字列からOPTYPEへの変換
let toOptype(s:string) : OPTYPE =
    match s with 
    | "Abs" -> Abs
    | "Add" -> Add
    | "And" -> And
    | "ArgMax" -> ArgMax
    | "ArgMin" -> ArgMin
    | "AveragePool" -> AveragePool
    | "BatchNormalization" -> BatchNormalization
    | "Cast" -> Cast
    | "Ceil" -> Ceil
    | "Clip" -> Clip
    | "Concat" -> Concat
    | "Constant" -> Constant
    | "Conv" -> Conv
    | "ConvTranspose" -> ConvTranspose
    | "DepthToSpace" -> DepthToSpace
    | "Div" -> Div
    | "Dropout" -> Dropout
    | "Elu" -> Elu
    | "Equal" -> Equal
    | "Exp" -> Exp
    | "Flatten" -> Flatten
    | "Floor" -> Floor
    | "GRU" -> GRU
    | "Gather" -> Gather
    | "Gemm" -> Gemm
    | "GlobalAveragePool" -> GlobalAveragePool
    | "GlobalLpPool" -> GlobalLpPool
    | "GlobalMaxPool" -> GlobalMaxPool
    | "Greater" -> Greater
    | "HardSigmoid" -> HardSigmoid
    | "Hardmax" -> Hardmax
    | "InstanceNormalization" -> InstanceNormalization
    | "LRN" -> LRN
    | "LSTM" -> LSTM
    | "LeakyRelu" -> LeakyRelu
    | "Less" -> Less
    | "Log" ->Log
    | "LogSoftmax" -> LogSoftmax
    | "LpNormalization" -> LpNormalization
    | "LpPool" -> LpPool
    | "MatMul" -> MatMul
    | "Max" -> Max
    | "MaxPool" -> MaxPool
    | "MaxRoiPool" -> MaxRoiPool
    | "Mean" -> Mean
    | "Min" -> Min
    | "Mul" -> Mul
    | "Neg" -> Neg
    | "Not" -> Not
    | "Or" -> Or
    | "PRelu" -> PRelu
    | "Pad" -> Pad
    | "Pow" -> Pow
    | "RNN" -> RNN
    | "RandomNormal" -> RandomNormal
    | "RandomNormalLike" -> RandomNormalLike
    | "RandomUniform" -> RandomUniform
    | "RandomUniformLike" -> RandomUniformLike
    | "Reciprocal" -> Reciprocal
    | "ReduceL1" -> ReduceL1
    | "ReduceL2" -> ReduceL2
    | "ReduceLogSum" -> ReduceLogSum
    | "ReduceLogSumExp" -> ReduceLogSumExp
    | "ReduceMax" -> ReduceMax
    | "ReduceMean" -> ReduceMean
    | "ReduceMin" -> ReduceMin
    | "ReduceProd" -> ReduceProd
    | "ReduceSum" -> ReduceSum
    | "ReduceSumSquare" -> ReduceSumSquare
    | "Relu" -> Relu
    | "Reshape" -> Reshape
    | "Selu" -> Selu
    | "Shape" -> Shape
    | "Sigmoid" -> Sigmoid
    | "Size" -> Size
    | "Slice" -> Slice
    | "Softmax" -> Softmax
    | "Softplus" -> Softplus
    | "Softsign" -> Softsign
    | "SpaceToDepth" -> SpaceToDepth
    | "Split" -> Split
    | "Sqrt" -> Sqrt
    | "Squeeze" -> Squeeze
    | "Sub" -> Sub
    | "Sum" -> Sum
    | "Tanh" -> Tanh
    | "Tile" -> Tile
    | "TopK" -> TopK
    | "Transpose" -> Transpose
    | "Unsqueeze" -> Unsqueeze
    | "Xor" -> Xor
    | "DUMMY" -> DUMMY 
    | _ -> raise(ParseError(s))

let toOtype(s : string) : OTPYE =
    match s with
    | "INT" -> INT
    | "INTS" -> INTS
    | _ -> raise(ParseError(s))

let make_attribute_ints(name : string, ints : int list) : ATTRIBUTE =
    match name with
    | "kernel_shape" -> KERNEL_SHAPE(ints)
    | "strides" -> STRIDES(ints)    
    | "pads" -> PADS(ints)
    | "dilations"  -> DILATIONS(ints)
    | _ -> raise(ParseError(name))

let make_attribute_int(name : string, i: int ) : ATTRIBUTE =
    match name with
    | "group"  -> GROUP(i)
    | "broadcast" -> BROADCAST(i)
    | "aixs" -> AIXS(i)
    | _ -> raise(ParseError(name))
      
//文字列の両側の""を取る関数。実装が雑
let removeDQs(s:string) : string =
  let l = String.length s
  s.Substring(1,l-2)


let parse_net_core(ts:TokenStream, g : OGRAPH) : OGRAPH =
    let mutable node = init_onode
    let mutable t = ts.get()
    let mutable op_type_done = false
    let mutable graph = g

    while t <> "" do
        match t with
        | "input:" -> 
            if op_type_done then
                graph <- graph_add_new_node(graph, node)
                op_type_done <- false
                node <- init_onode
            let v = ts.get() |> removeDQs |> int
            node <- node_add_inputs(node, v)
        | "output:" ->
            let v = ts.get() |> removeDQs |> int
            node <- node_add_outputs(node, v)
        | "op_type:" ->
            op_type_done <- true
            let op = ts.get() |> removeDQs |> toOptype
            node <- node_set_optype(node, op)
        | "doc_string:" ->
            let ds = ts.get() |> removeDQs
            node <- node_set_docstr(node, ds)
        | "attribute" ->
            ts.get() |> ignore // '{' を捨てる。
            let name_e = ts.get() 
            if name_e <> "name:" then
                raise(ParseError(name_e))
            let name = ts.get() |> removeDQs
            match name with 
            | "kernel_shape" | "strides" | "pads" | "dilations"  -> 
                let mutable ints : int list = []
                let mutable at = ts.get()
                while at <> "type:" do
                    match at with
                    | "ints:" ->             
                        let v = ts.get() |> int
                        ints <- ints @ [v]
                        at <- ts.get()
                    | _ -> raise (ParseError(at))

                let type_s = ts.get() |> toOtype
                let attr = make_attribute_ints(name, ints)
                node <- node_add_attributes(node, attr)
                ts.get() |> ignore // '}' を捨てる。
            |   "group" | "broadcast" | "aixs" ->
                ts.get() |> ignore // iを捨てる
                let i = ts.get() |> int
                let attr = make_attribute_int(name, i)
                node <- node_add_attributes(node, attr)
                ts.get() |> ignore // '}' を捨てる。
            | _ -> raise (ParseError(name))
         
        | _ -> raise(ParseError(t))

        t <- ts.get()

    graph_add_new_node(g, node)
                                                                                      

let parse_net (ts : string list) : OGRAPH =
    let g = init_graph()
    let token_strem = TokenStream(ts)
    parse_net_core(token_strem, g)
