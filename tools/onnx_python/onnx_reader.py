import os
import onnx


DATA_DIR = '../../data/'
tyolo_model = 'tiny_yolov2/model.onnx'


def model_read(onnx_file: str):
    model = onnx.ModelProto()
    with open(onnx_file, 'rb') as fp:
        content = fp.read()
        model.ParseFromString(content)
    return model


def main():
    onnx_file: str = os.path.join(DATA_DIR, tyolo_model)
    model = model_read(onnx_file)

    for op_id, op in enumerate(model.graph.node):
        print(op_id, " ", op)


if __name__ == '__main__':
    main()

