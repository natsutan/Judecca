use std::cell::RefCell;
use std::collections::HashSet;

use std::fs::File;
use std::io::{Write, BufWriter};


use crate::onnx_io::onnx;
use std::string::ToString;
//use protobuf::rt::vec_packed_enum_data_size;
//use core::borrow::BorrowMut;

type Link = RefCell<IrNode>;

#[derive(Debug, Clone)]
pub struct IrNode {
    pub name: String,
    pub succ: Vec<Link>,
    pub pred: Vec<Link>,
    pub use_s: HashSet<String>,
    pub def_s: HashSet<String>,
    pub in_s: HashSet<String>,
    pub out_s: HashSet<String>
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
        let irnode = IrNode::new(node_name);
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
    for cur_node in &ir {
        let outs = cur_node.borrow().def_s.clone();
        for out in outs {
            for next_node in &ir {
                if next_node.borrow_mut().use_s.contains(&out) {
                    cur_node.borrow_mut().succ.push(next_node.clone());
                    next_node.borrow_mut().pred.push(cur_node.clone())
                }
            }
        }
    }
    ir
}

fn is_table_same(g0:& LivenessIr, in_s : &Vec<HashSet<String>>, out_s: &Vec<HashSet<String>>) -> bool {
    assert!(g0.len() == in_s.len());
    assert!(g0.len() == out_s.len());

    let mut result:bool = true;

    for i in 0 .. g0.len() {
        if g0[i].borrow().in_s != in_s[i] {
            result = false;
        }
//        if g0[i].out_s != out_s[i] {
//            result = false;
//        }
    }


    result
}

pub fn liveness_analysis(ir: &mut LivenessIr) {
    let mut in_s_save : Vec<HashSet<String>> = vec![];
    let mut out_s_save : Vec<HashSet<String>> = vec![];

    //copy
    let mut first = true;

    for _i in 0..ir.len() {
        let mut new_use_s:HashSet<String> = HashSet::new();
        //loopをdo while相当にするために、最初にDummyを入れて比較を失敗させる。
        if first {
            new_use_s.insert("Dummy".to_string());
            first = false;
        }

        in_s_save.push(new_use_s);
        out_s_save.push(HashSet::new())

    }


    while !is_table_same(&ir, &in_s_save, &out_s_save) {
        print!("loop\n");
        in_s_save[0].remove(&"Dummy".to_string());
    }

}

pub fn write_dot(ir:LivenessIr, file:String) {
    let mut writer = BufWriter::new(File::create(file).unwrap());
    writer.write("digraph livness{\n".as_bytes()).unwrap();
    for node in ir {
        let name = &node.borrow().name;
        for out in &node.borrow().succ {
            let dst = &out.borrow().name;
            let line = format!("{} -> {};\n", name, dst);
            writer.write(line.as_bytes()).unwrap();
        }
    }


    writer.write("}\n".as_bytes()).unwrap();
}