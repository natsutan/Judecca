module parser_Test

open NUnit.Framework
open parser


[<TestFixture>]
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

