
extern crate protobuf;

use std::fs::File;
use std::io::{BufReader};
use protobuf::{CodedInputStream, Message};

use crate::onnx_io::onnx;

pub fn onnx_read(onnx_path: &str) ->onnx::ModelProto {
    let file = File::open(onnx_path).expect("fail to open file");
    let mut buffered_reader = BufReader::new(file);
    let mut cis = CodedInputStream::from_buffered_reader(&mut buffered_reader);

    let mut u = onnx::ModelProto::new();
    u.merge_from(&mut cis).expect("fail to merge");
    u
}




