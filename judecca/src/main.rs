extern crate protobuf;

use std::fs::File;
use std::io::{BufReader};
use protobuf::{CodedInputStream, Message};
use judecca::onnx_io::onnx_reader::onnx_read;

//protoc --rust_out . onnx.proto で生成されたonnx.rsを読み込む

fn main() {
    let mut u = onnx_read("../data/squeezenet/model.onnx");

    println!("producer name: {}", u.get_producer_name());

}
