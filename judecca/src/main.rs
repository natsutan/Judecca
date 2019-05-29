extern crate protobuf;

use std::fs::File;
use std::io::{BufReader};
use protobuf::{CodedInputStream, Message};
use judecca::onnx_io::onnx_reader::onnx_read;
use judecca::liveness::liveness_analysis::onnx_to_ir;

//protoc --rust_out . onnx.proto で生成されたonnx.rsを読み込む

fn main() {
    let mut u = onnx_read("../data/squeezenet/model.onnx");

    println!("producer name: {}", u.get_producer_name());

    let ir = onnx_to_ir(&u);

    for i in &ir{
        println!("judecca node{:?}", i);
    }


}
