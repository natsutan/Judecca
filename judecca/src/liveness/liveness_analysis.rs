use std::collections::HashSet;

use crate::onnx_io::onnx;

pub struct Node {
    name: str,
    succ: List<i32>,
    pred: List<i32>,
    use_s: HashSet<str>,
    def_s: HashSet<str>,
    in_s: HashSet<str>,
    out_s: HashSet<str>
}

pub struct LivenessIr {
    node: array<Node>
}

pub fn onnx_to_ir(onnx:onnx::ModelProto) -> LivenessIr {


}

pub fn livness_analisys(ir: &mut LivenessIr) {




}