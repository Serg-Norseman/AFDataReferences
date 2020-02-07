using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AFBitmaskDR
{
    [TestFixture]
    public class BitmaskDRTests
    {
        [Test]
        public void Test_GetResource()
        {
            string str = Resources.ERR_AttributeHasNotBeenSet;
            Assert.AreEqual("The attribute has not been set for '{0}' data reference.", str);
        }

        [Test]
        public void Test_GetBit()
        {
            Assert.AreEqual(1, BitmaskCore.GetBit(277, 0)); // 0b100010101
            Assert.AreEqual(0, BitmaskCore.GetBit(277, 1)); // 0b100010101
            Assert.AreEqual(1, BitmaskCore.GetBit(277, 2)); // 0b100010101
            Assert.AreEqual(0, BitmaskCore.GetBit(277, 3)); // 0b100010101
            Assert.AreEqual(1, BitmaskCore.GetBit(277, 4)); // 0b100010101
            Assert.AreEqual(0, BitmaskCore.GetBit(277, 5)); // 0b100010101

            Assert.AreEqual(0, BitmaskCore.GetBit(277, 7)); // 0b100010101
            Assert.AreEqual(1, BitmaskCore.GetBit(277, 8)); // 0b100010101
        }
    }
}