
struct Node {
    name: String,
    layer_type: Layer,
    attribute: HashMap<String, String>
};

enum Layer {
    MaxPool,
    Conv,
    Relu,
    Concat,
    GlobalAveragePool,
    Softmax
};