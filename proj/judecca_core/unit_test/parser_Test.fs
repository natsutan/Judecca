module parser_Test

open NUnit.Framework
open parser


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


[<TestFixture>]
[<Category("Parser")>]
type ParserTest () = 
    class
        [<Test>]
        member public this.parser_test0() =
            let test_ts0 = TokenStream(["input:"; "\"1\"";"op_type:"; "\"Conv\""])
            let expecetd_0 : ONODE = {
                inputs=[1] ; outputs = [] ; optype = Conv ; doc_string = "" ; attributes = [] 
                }
            let g = parser.parse_net_core(test_ts0, [])
            Assert.AreEqual(List.length g, 1)
            Assert.AreEqual(List.head g, expecetd_0)

        [<Test>]
        member public this.parser_test1() =
            let test_ts0 = TokenStream(
                ["input:"; "\"1\"";
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
            let g = parser.parse_net_core(test_ts0, [])
            Assert.AreEqual(List.length g, 1)
            Assert.AreEqual(List.head g, expected_0)

    end