using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AFTransformerDR
{
    [TestFixture]
    public class TransformerDRTests
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
        }
    }
}