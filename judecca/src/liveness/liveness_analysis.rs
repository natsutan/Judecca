use std::cell::{Ref, RefCell};
use std::rc::Rc;
use std::collections::HashSet;

use crate::onnx_io::onnx;
use protobuf::rt::vec_packed_enum_data_size;
use core::borrow::{Borrow, BorrowMut};

type Link = RefCell<IrNode>;

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
    pub fn new(name: String) ->RefCell<IrNode> {
        RefCell::new(IrNode {
            name: name,
            succ: vec![],
            pred: vec![],
            use_s: HashSet::new(),
            def_s: HashSet::new(),
            in_s: HashSet::new(),
            out_s: HashSet::new()
        })
    }
    pub fn add_output(& mut self, s:&String) {
        self.use_s.insert(s.clone());
    }

}


type LivenessIr = Vec<RefCell<IrNode>>;

pub fn onnx_to_ir(onnx :&onnx::ModelProto) -> LivenessIr {
    let mut ir :LivenessIr = vec![];
    let g = onnx.get_graph();
    let mut layer_cnt = 0;

    for onnx_node in g.get_node() {
        let node_name = format!("{}_{}", onnx_node.get_op_type().to_string(), layer_cnt);
        // output -> def, input -> use
        let mut irnode = IrNode::new(node_name);
        for output in onnx_node.get_output() {
            irnode.borrow_mut().def_s.insert(output.clone());
        }
        for input in onnx_node.get_input() {
            irnode.borrow_mut().use_s.insert(input.clone());
        }

        ir.push(irnode);

        layer_cnt = layer_cnt + 1;
    }

    //make graph
//    for mut cur_node in ir {
//        for out in cur_node.borrow_mut().def_s.clone() {
//            for mut next_node in ir.borrow_mut() {
//                if next_node.borrow_mut().use_s.contains(&out) {
//                    cur_node.borrow_mut().succ.push(next_node);
//                }
//            }
//        }
//    }


    ir
}

pub fn livness_analisys(ir: &mut LivenessIr) {




}