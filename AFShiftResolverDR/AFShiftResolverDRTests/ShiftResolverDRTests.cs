using System;
using System.Globalization;
using NUnit.Framework;
using OSIsoft.AF.Time;

namespace AFShiftResolverDR
{
    [TestFixture]
    public class ShiftResolverDRTests
    {
        private static DateTime ParseDT(string str)
        {
            return DateTime.ParseExact(str, "yyyy.MM.dd HH:mm:ss", CultureInfo.InvariantCulture);
        }

        
        [Test]
        public void Test_GetResource()
        {
            string str = Resources.ERR_AttributeHasNotBeenSet;
            Assert.AreEqual("The attribute has not been set for '{0}' data reference.", str);
        }

        [Test]
        public void Test_TryGetShiftMode()
        {
            int result = SRHelper.TryGetShiftMode("111");
            Assert.AreEqual(111, result);

            result = SRHelper.TryGetShiftMode(@".\ShiftModeAttr");
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void Test_GetWorkShift()
        {
            int workShift = SRHelper.GetWorkShift(DateTime.Parse("2021.04.22 23:30:00"), 3, -60);
            Assert.AreEqual(1, workShift);

            workShift = SRHelper.GetWorkShift(DateTime.Parse("2021.04.23 07:30:00"), 3, -60);
            Assert.AreEqual(2, workShift);

            workShift = SRHelper.GetWorkShift(DateTime.Parse("2021.04.22 15:30:00"), 3, -60);
            Assert.AreEqual(3, workShift);
        }

        [Test]
        public void Test_GetWorkShiftParams()
        {
            double[] wsParams;
            DateTime dtx;

            dtx = ParseDT("2021.04.22 23:30:00");
            wsParams = SRHelper.GetWorkShiftParams(dtx, 3, -60);
            Assert.AreEqual(1.00d, wsParams[0]);
            Assert.AreEqual(new AFTime("2021.04.22 23:00:00"), new AFTime(wsParams[1]));
            Assert.AreEqual(new AFTime("2021.04.23 07:00:00"), new AFTime(wsParams[2]));

            dtx = ParseDT("2021.04.23 08:30:00");
            wsParams = SRHelper.GetWorkShiftParams(dtx, 3, -60);
            Assert.AreEqual(2.00d, wsParams[0]);
            Assert.AreEqual(new AFTime("2021.04.23 07:00:00"), new AFTime(wsParams[1]));
            Assert.AreEqual(new AFTime("2021.04.23 15:00:00"), new AFTime(wsParams[2]));

            dtx = ParseDT("2021.04.22 21:30:00");
            wsParams = SRHelper.GetWorkShiftParams(dtx, 3, -60);
            Assert.AreEqual(3.00d, wsParams[0]);
            Assert.AreEqual(new AFTime("2021.04.22 15:00:00"), new AFTime(wsParams[1]));
            Assert.AreEqual(new AFTime("2021.04.22 23:00:00"), new AFTime(wsParams[2]));
        }
    }
}