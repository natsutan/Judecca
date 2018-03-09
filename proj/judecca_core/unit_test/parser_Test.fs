module parser_Test

open NUnit.Framework
open tokenizer
open parser_net
open parser_input

[<TestFixture>]
[<Category("TokenStream")>]
type TokenStreamTest () = 
    class
        [<Test>]
        member public this.TokenStream_test0() =
            let ts0 = TokenStream(["x"])
            Assert.AreEqual(ts0.get(), "x")
            Assert.True(ts0.is_last())
            Assert.AreEqual(ts0.get(), "")

        [<Test>]
        member public this.TokenStream_test1() =
            let ts0 = TokenStream(["x";"y"])
            Assert.AreEqual(ts0.get(), "x")
            Assert.False(ts0.is_last())
            Assert.AreEqual(ts0.get(), "y")
            Assert.True(ts0.is_last())
            Assert.AreEqual(ts0.get(), "")
    end

[<TestFixture>]
[<Category("ONODE")>]
type OnodeTest () = 
    class
        [<Test>]
        member public this.node_test0() =
            let onode = init_onode
            // inputs
            let r0 = node_add_inputs(onode, 3)
            let expected_node0 = {inputs=[3] ; 
                                outputs=[]; optype=DUMMY; doc_string=""; attributes=[] }
            Assert.AreEqual(r0, expected_node0)
            let r1 = node_add_inputs(r0, 2)
            let expected_node1 = {inputs=[3;2] ; 
                                outputs=[]; optype=DUMMY; doc_string=""; attributes=[] }
            Assert.AreEqual(r1, expected_node1)

            // outputs
            let r2 = node_add_outputs(r1, 4)
            let expected_node2 = {inputs=[3;2] ; outputs=[4]; 
                                optype=DUMMY; doc_string=""; attributes=[] }
            Assert.AreEqual(r2, expected_node2)
            let r3 = node_add_outputs(r2, 5)
            let expected_node3 = {inputs=[3;2] ; outputs=[4;5]; 
                                optype=DUMMY; doc_string=""; attributes=[] }
            Assert.AreEqual(r3, expected_node3)

        [<Test>]
        member public this.node_test1() =
            let onode = init_onode
            let r0 = node_set_optype(onode, Conv)
            let expected_node0 = {inputs=[] ; outputs=[]; 
                                optype=Conv; doc_string=""; attributes=[] }
            Assert.AreEqual(r0, expected_node0)

            let r1 = node_set_docstr(r0, "abc")
            let expected_node1 = {inputs=[] ; outputs=[]; 
                                optype=Conv; doc_string="abc"; attributes=[] }
            Assert.AreEqual(r1, expected_node1)

        [<Test>]
        member public this.node_test2() =
            let onode = init_onode
            let attr0 = make_attribute_ints("kernel_shape", [1;2;3])
            let attr1 = make_attribute_int("group", 4)
            
            let r0 = node_add_attributes(onode, attr0)
            let expected_node0 = {inputs=[] ; outputs=[]; 
                                optype=DUMMY; doc_string=""; attributes=[attr0] }
            Assert.AreEqual(r0, expected_node0)

            let r1 = node_add_attributes(r0, attr1)
            let expected_node1 = {inputs=[] ; outputs=[]; 
                                optype=DUMMY; doc_string=""; attributes=[attr0;attr1] }
            Assert.AreEqual(r1, expected_node1)


    end

let input_text0 = "input: \"1\"
input: \"2\"
output: \"55\"
op_type: \"Conv\"
attribute {
  name: \"kernel_shape\"
  ints: 3
  ints: 3
  type: INTS
}
attribute {
  name: \"strides\"
  ints: 2
  ints: 2
  type: INTS
}
attribute {
  name: \"pads\"
  ints: 0
  ints: 0
  ints: 0
  ints: 0
  type: INTS
}
attribute {
  name: \"dilations\"
  ints: 1
  ints: 1
  type: INTS
}
attribute {
  name: \"group\"
  i: 1
  type: INT
}
"
let input_text1 = "input: \"162\"
output: \"163\"
op_type: \"Relu\"
doc_string: \"abc\"
"

let input_text2 = "input: \"159\"
input: \"163\"
output: \"164\"
op_type: \"Concat\"
attribute {
  name: \"axis\"
  i: 1
  type: INT
}
doc_string: \"abc\"
"
let input_text3 = "input: \"164\"
output: \"166\"
output: \"167\"
op_type: \"Dropout\"
attribute {
  name: \"is_test\"
  i: 1
  type: INT
}
attribute {
  name: \"ratio\"
  f: 0.5
  type: FLOAT
}
"

[<TestFixture>]
[<Category("ParserNet")>]
type ParserNetTest () = 
    class
        [<Test>]
        member public this.parser_net_test0() =
            let test_ts0 = TokenStream(["input:"; "\"1\"";"op_type:"; "\"Conv\""])
            let expecetd_0 : ONODE = {
                inputs=[1] ; outputs = [] ; optype = Conv ; doc_string = "" ; attributes = [] 
                }
            let g = parser_net.parse_net_core(test_ts0, [])
            Assert.AreEqual(List.length g, 1)
            Assert.AreEqual(List.head g, expecetd_0)

        [<Test>]
        member public this.parser_net_test1() =
            let test_ts0 = TokenStream(["input:"; "\"1\"";
            "output:"; "\"55\"";
            "op_type:"; "\"Conv\"";
            "doc_string:" ; "\"abc\"" ;
            "attribute" ; "{" ; "name:" ; "\"kernel_shape\"" ; "ints:" ; "3" ; "ints:" ; "3" ;"type:" ; "INTS" ; "}"
            ])
            let expected_attr : ATTRIBUTE = make_attribute_ints( "kernel_shape", [3;3])
            let expected_0 : ONODE = {
                inputs=[1] ; outputs = [55] ; optype = Conv ; doc_string = "abc" ; 
                attributes = [expected_attr] 
                }
            let g = parser_net.parse_net_core(test_ts0, [])
            Assert.AreEqual(List.length g, 1)
            Assert.AreEqual(List.head g, expected_0)

        [<Test>]
        member public this.parser_net_test2() =
            let g = input_text0 |> tokenize |> parse_net
            Assert.AreEqual(List.length g, 2)
            let g = input_text1 |> tokenize |> parse_net
            Assert.AreEqual(List.length g, 2)
            let g = input_text2 |> tokenize |> parse_net
            Assert.AreEqual(List.length g, 2)
            let g = input_text3 |> tokenize |> parse_net
            Assert.AreEqual(List.length g, 2)

    end

[<TestFixture>]
[<Category("ParserInput")>]
type ParserInput () = 
    class
        [<Test>]
        member public this.parser_input_test0() =
            let test0_ts = TokenStream(["name:" ;"\"1\""])
            let name = parse_name test0_ts
            Assert.AreEqual(name, "1")
            Assert.IsTrue(test0_ts.is_last())

            let test1_ts = TokenStream(["elem_type:"; "FLOAT"])
            let etype = parse_elem_type(test1_ts)
            Assert.AreEqual(etype, FLOAT)
            Assert.IsTrue(test1_ts.is_last())

            let test2_ts = TokenStream(["shape"; "{";
            "dim"; "{" ;"dim_value:"; "1"; "}" ;
            "dim"; "{" ;"dim_value:"; "3"; "}" ;
            "dim"; "{" ;"dim_value:"; "224"; "}" ;
            "dim"; "{" ;"dim_value:"; "224";  "}" ;
            "}"; "}"])
            let shape = parse_shape test2_ts
            Assert.AreEqual(shape, [1;3;224;224])
            Assert.IsTrue(test2_ts.is_last())
    end