mod onnx_reader;

fn main() {
    let x = onnx_reader::onnx_reader::onnxread();
    
    println!("Hello, world! {}", x);
}

