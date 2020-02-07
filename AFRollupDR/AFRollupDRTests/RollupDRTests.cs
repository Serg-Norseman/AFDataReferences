using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace AFRollupDR
{
    [TestFixture]
    public class RollupDRTests
    {
        [Test]
        public void Test_GetResource()
        {
            string str = Resources.ERR_AttributeHasNotBeenSet;
            Assert.AreEqual("The attribute has not been set for '{0}' data reference.", str);
        }
    }
}