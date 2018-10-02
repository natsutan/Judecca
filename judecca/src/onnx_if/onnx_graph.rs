use std::fs;
use std::io::{BufWriter, Write};

use onnx_if::onnx::ModelProto;

pub fn node_dump(model:&ModelProto) {

    let graph = model.get_graph();
    let nodes = graph.get_node();

    for node in nodes {
        println!("node: {:?}", node);
    }
}

pub fn write_dot(model:&ModelProto, filename:&str) {
    //ファイルを開く
    let fp = fs::File::create(filename).unwrap();
    let mut fb = BufWriter::new(fp);
    //グラフ全体の情報を取る
    let graph_name = get_model_name(model);
    fb.write(graph_name.as_bytes()).unwrap();


    //ノードの一覧を出力
    let mut name_gen = NodeNameGenerator{num :0, prefix :&"OP".to_string()};
    println!("op_name = {}", generate_name(&mut name_gen));
    println!("op_name = {}", generate_name(&mut name_gen));
    println!("op_name = {}", generate_name(&mut name_gen));


    //接続を出力

}

fn get_model_name(model:&ModelProto) -> &str {
    model.get_graph().get_name()
}

//#[derive(Sized, Clone)]
struct NodeNameGenerator<'a> {
    num: u64,
    prefix: &'a String
}


fn generate_name(gen:&mut NodeNameGenerator) -> String{
    let name = format!("{}{}", gen.prefix, gen.num);
    gen.num = gen.num + 1;
    name
}

