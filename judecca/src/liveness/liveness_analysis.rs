use std::cell::{Ref, RefCell};
use std::rc::Rc;
use std::collections::HashSet;

use crate::onnx_io::onnx;
use protobuf::rt::vec_packed_enum_data_size;

type Link = Option<Rc<RefCell<IrNode>>>;

#[derive(Debug, Clone)]
pub struct IrNode {
    name: String,
    succ: Vec<Link>,
    pred: Vec<Link>,
    use_s: HashSet<String>,
    def_s: HashSet<String>,
    in_s: HashSet<String>,
    out_s: HashSet<String>
}

impl IrNode {
    pub fn new(name: String) -> IrNode {
        IrNode {
            name: name,
            succ: vec![],
            pred: vec![],
            use_s: HashSet::new(),
            def_s: HashSet::new(),
            in_s: HashSet::new(),
            out_s: HashSet::new()
        }
    }
}


type LivenessIr = Vec<IrNode>;

pub fn onnx_to_ir(onnx :&onnx::ModelProto) -> LivenessIr {
    let mut ir :LivenessIr = vec![];
    let g = onnx.get_graph();
    let mut layer_cnt = 0;

    for onnx_node in g.get_node() {
        let node_name = format!("{}_{}", onnx_node.get_op_type().to_string(), layer_cnt);
        let mut irnode = IrNode::new(node_name);
        ir.push(irnode.clone());

        layer_cnt = layer_cnt + 1;
    }


    ir
}

pub fn livness_analisys(ir: &mut LivenessIr) {




}