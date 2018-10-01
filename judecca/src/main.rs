extern crate protobuf;

//protoc --rust_out . onnx.proto で生成されたonnx.rsを読み込む
mod onnx_reader;

fn main() {
    let model:onnx_reader::onnx::ModelProto = onnx_reader::onnx_reader::onnxread("../data/squeezenet/model.onnx");
    println!("producer name: {}", model.get_producer_name());

}

