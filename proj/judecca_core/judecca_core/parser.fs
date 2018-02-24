module parser

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

//onnxの属性
type OAttribute(name:string) =
    class
        let name = name
        let mutable elem = []
    end

//onnx出力の構成要素
type Oelm = 
    | OInput of int
    | OOutput of int
    | OOp_type of OPTYPE
    | OAttribute of OAttribute


