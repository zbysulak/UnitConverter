using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitConverter.Tests
{
    [TestClass()]
    public class PrefixesTests
    {
        private Prefixes prefixes;
        public PrefixesTests() {
            prefixes = new Prefixes();
        }

        [TestMethod()]
        public void GetBaseUnitTest()
        {
            Assert.AreEqual(prefixes.GetBaseUnit("kilometer"), "meter");
            Assert.AreEqual(prefixes.GetBaseUnit("meter"), "meter");
            Assert.AreEqual(prefixes.GetBaseUnit("noprefixmeter"), "noprefixmeter");
        }

        [TestMethod()]
        public void NormalizeTest()
        {
            Assert.AreEqual(prefixes.Normalize(new ValueUnit { Value = 1, Unit = "kilometer" }),
                new ValueUnit { Value = 1000, Unit = "meter" });
        }

        [TestMethod()]
        public void DenormalizeTest()
        {
            Assert.AreEqual(prefixes.Denormalize(1, "kilometer"),
                new ValueUnit { Value = 0.001, Unit = "kilometer" });
        }
    }
}