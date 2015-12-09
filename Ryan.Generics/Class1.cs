using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ryan.Generics
{

    [TestClass]
    public class TestRyanDelegate
    {
        [TestMethod]
        public void TestGeneric()
        {
            var ryan = new Ryan<string>("hi");
            //var ryan2 = new Ryan<string>(100);  // This will fail because I am declaring T to string in Ryan<string>, so the param must be a string
            Assert.AreEqual("hi", ryan.Foo());

            // BUT - if I declare T to be an INT, then I can pass in an int
            var ryan2 = new Ryan<int>(100);
            Assert.AreEqual(100, ryan2.Foo());
        }
    }


    public class Ryan<T>  // T generic  declaration - if I change T to T2 then all references to T must change to T2
    {
        T _greeting; // Must match the type declared in the class name

        public Ryan(T greeting)
        {
            _greeting = greeting;  // Must match the type declared in the class name
        }

        public T Foo()  // Must match the type declared in the class name
        {
            return _greeting;
        }
    }
}
