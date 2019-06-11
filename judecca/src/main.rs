extern crate protobuf;
use std::ops::Deref;
//use std::fs::File;
//use std::io::{BufReader};
//use protobuf::{,Message};
use judecca::onnx_io::onnx_reader::onnx_read;
use judecca::liveness::liveness_analysis::{onnx_to_ir, write_dot, liveness_analysis, print_liveness};

//protoc --rust_out . onnx.proto で生成されたonnx.rsを読み込む

fn main() {
    let u = onnx_read("../data/squeezenet/model.onnx");

    println!("producer name: {}", u.get_producer_name());

    let mut ir = onnx_to_ir(&u);

    liveness_analysis(&mut ir);


    for node in &ir  {
        let name = &node.borrow().name;
        let mut succ_str = "".to_string();
        for s in node.borrow().clone().succ {
            succ_str = format!("{} {}", succ_str, s.borrow().name);
        }

        let mut pred_str = "".to_string();
        for p in node.borrow().clone().pred {
            pred_str = format!("{} {} ", pred_str, p.borrow().name);
        }

        let use_s = format!("{:?}", node.borrow().use_s);
        let def_s = format!("{:?}", node.borrow().def_s);

        let line = format!("node:{} succ:[{}] pred:[{}] u:{} d:{}", name, succ_str, pred_str, use_s, def_s);
        print!("{}\n", line);
    }

    print_liveness(&ir);

    //write_dot(ir, "output/liveness.dot".to_string());

}
