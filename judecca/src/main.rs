extern crate protobuf;

use std::fs::File;
use std::io::{BufReader};
use protobuf::{CodedInputStream, Message};

//protoc --rust_out . onnx.proto で生成されたonnx.rsを読み込む
mod onnx_reader;

fn main() {
    let file = File::open("../data/squeezenet/model.onnx").expect("fail to open file");
    let mut buffered_reader = BufReader::new(file);
    let mut cis = CodedInputStream::from_buffered_reader(&mut buffered_reader);

    let mut u = onnx_reader::onnx::ModelProto::new();
    u.merge_from(&mut cis).expect("fail to merge");

    println!("producer name: {}", u.get_producer_name());

}
