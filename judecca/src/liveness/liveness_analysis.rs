use std::cell::RefCell;
use std::collections::HashSet;

use std::fs::File;
use std::io::{Write, BufWriter};


use crate::onnx_io::onnx;
use std::string::ToString;
use core::borrow::Borrow;

#[derive(Debug, Clone)]
pub struct IrNode {
    pub name: String,
    pub id: u32,
    pub succ: Vec<u32>,
    pub pred: Vec<u32>,
    pub use_s: HashSet<String>,
    pub def_s: HashSet<String>,
    pub in_s: HashSet<String>,
    pub out_s: HashSet<String>
}

impl IrNode {
    pub fn new(name: String, id: u32) ->RefCell<IrNode> {
        RefCell::new(IrNode {
            name: name,
            id: id,
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
    let onnx_graph = onnx.get_graph();
    let mut layer_cnt = 0;
    let mut symtbl:HashSet<String> = HashSet::new();

    for onnx_node in onnx_graph.get_node() {
        let node_name = format!("{}_{}", onnx_node.get_op_type().to_string(), layer_cnt);
        // output -> def, input -> use
        let irnode = IrNode::new(node_name, layer_cnt);
        for output in onnx_node.get_output() {
            irnode.borrow_mut().def_s.insert(output.clone());
            symtbl.insert(output.clone());
        }

        //inputに存在しないuseはここでdefにする。
        let mut extra_def_s:HashSet<String> = HashSet::new();
        for input in onnx_node.get_input() {
            irnode.borrow_mut().use_s.insert(input.clone());
            if !symtbl.contains(&input.to_string()) {
                extra_def_s.insert(input.clone());
                symtbl.insert(input.clone());
            }

        }
        for extra_def in extra_def_s {
            irnode.borrow_mut().def_s.insert(extra_def);
        }

        ir.push(irnode);
        layer_cnt = layer_cnt + 1;
    }

    //make graph
    for i in 0 .. ir.len() {
        let outs =  ir[i].borrow().def_s.clone();
        for out in outs {
            for j in 0 .. ir.len() {
                if i == j {
                    continue;
                }
                let mut cur_node = ir[i].borrow_mut();
                let mut next_node = ir[j].borrow_mut();
                let use_s_clone = next_node.borrow().use_s.clone();
                if use_s_clone.contains(&out) {
                    cur_node.succ.push(next_node.borrow().id.clone());
                    next_node.pred.push(cur_node.borrow().id.clone())
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
//     print_liveness(&g0);

    for i in 0 .. g0.len() {
        if g0[i].borrow().in_s != in_s[i] {
            result = false;
        }
        if g0[i].borrow().out_s != out_s[i] {
            result = false;
        }
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

    //livness analysis本体
    while !is_table_same(&ir, &in_s_save, &out_s_save) {
        //ダミーを取り除く
        in_s_save[0].remove(&"Dummy".to_string());
        //データのコピー
        for i in 0..ir.len() {
            in_s_save[i] = ir[i].borrow().in_s.clone();
            out_s_save[i] = ir[i].borrow().out_s.clone();

            let mut diff: HashSet<String> = HashSet::new();
            for d in ir[i].borrow().out_s.difference(&ir[i].borrow().def_s) {
                diff.insert(d.clone());
            }
            let mut union: HashSet<String> = HashSet::new();
            for u in ir[i].borrow().use_s.union(&diff) {
                union.insert(u.to_string());
            }

            //今のノードで定義しているものを除く（重みデータは固定値）
            for cur_in in &ir[i].borrow().def_s {
                union.remove(&cur_in.to_string());
            }
            ir[i].borrow_mut().in_s = union;

            let mut succ_in_union:HashSet<String> = ir[i].borrow_mut().out_s.clone();

            if i < ir.len()-1{
                for succ_in in &ir[i+1].borrow().in_s {
                    succ_in_union.insert(succ_in.to_string());
                }
            }
            ir[i].borrow_mut().out_s = succ_in_union;
        }
    }
}

pub fn print_liveness(ir: &LivenessIr) {
    print!("--- liveness ---\n");
    for node in ir  {
        let name = &node.borrow().name;

        let in_s = format!("{:?}", node.borrow().in_s);
        let out_s = format!("{:?}", node.borrow().out_s);

        let line = format!("{} in:{} out:{}", name, in_s, out_s);
        print!("{}\n", line);
    }

}

pub fn write_dot(ir:&LivenessIr, file:String) {
    let mut writer = BufWriter::new(File::create(file).unwrap());
    writer.write("digraph livness{\n".as_bytes()).unwrap();
    for node in ir {
        let name = &node.borrow().name;
        let out_node = &node.borrow().clone();
        for out in &out_node.succ {
            let dst = &ir[*out as usize].borrow().name;
            let line = format!("{} -> {};\n", name, dst);
            writer.write(line.as_bytes()).unwrap();
        }
    }

    writer.write("}\n".as_bytes()).unwrap();
}

pub fn write_csv(ir:&LivenessIr, file:String) {
    let mut writer = BufWriter::new(File::create(file).unwrap());
    writer.write("name, in, out\n".as_bytes()).unwrap();

    for node in ir {
        let name = &node.borrow().name;

        let mut in_s_line = "".to_string();
        for in_s in &node.borrow().in_s {
            in_s_line = format!("{} {}", in_s_line, in_s);
        }
        let mut out_s_line = "".to_string();
        for out_s in &node.borrow().out_s {
            out_s_line = format!("{} {}", out_s_line, out_s);
        }
        let line = format!("{},{},{}\n", name, in_s_line, out_s_line);
        writer.write(line.as_bytes()).unwrap();
    }

}