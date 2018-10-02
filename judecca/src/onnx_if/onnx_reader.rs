use std::fs::File;
use std::io::{BufReader};
use protobuf::{CodedInputStream, Message};

//protoc --rust_out . onnx.proto で生成されたonnx.rsを読み込む
use onnx_if::onnx::ModelProto;

pub fn onnxread(path:&str) -> ModelProto {
    let file = File::open(path).expect("fail to open file");
    let mut buffered_reader = BufReader::new(file);
    let mut cis = CodedInputStream::from_buffered_reader(&mut buffered_reader);

    let mut u = ModelProto::new();
    u.merge_from(&mut cis).expect("fail to merge");

    u
}








