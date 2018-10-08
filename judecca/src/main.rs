extern crate protobuf;

//protoc --rust_out . onnx.proto で生成されたonnx.rsを読み込む
mod onnx_if;

fn main() {
    let model:onnx_if::onnx::ModelProto = onnx_if::onnx_reader::onnxread("../data/squeezenet/model.onnx");
//    onnx_if::onnx_graph::node_dump(&model);
    onnx_if::onnx_graph::write_dot(&model, "sq.dot");

}

