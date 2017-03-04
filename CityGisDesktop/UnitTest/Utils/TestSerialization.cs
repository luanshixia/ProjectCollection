using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using Dreambuild.IO;

namespace UnitTest
{
    [Serializable]
    public class MyClass
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }

    [TestFixture]
    public class TestSerialization
    {
        [Test]
        public void TestBinaryEncode()
        {
            var data1 = new DateTime();
            var code1 = data1.BinaryEncode();
            var data2 = new MyClass { ID = 1, Name = "Yang" };
            var code2 = data2.BinaryEncode();
            Assert.AreEqual(code1.BinaryDecode<DateTime>(), data1);
            Assert.True(code2.BinaryDecode<MyClass>().Name == "Yang");
        }
    }
}
