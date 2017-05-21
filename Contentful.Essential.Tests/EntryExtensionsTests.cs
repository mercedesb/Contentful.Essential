using Contentful.Core.Models;
using Contentful.Essential.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contentful.Essential.Tests
{
    [TestClass]
    public class EntryExtensionsTests
    {
        [TestMethod]
        public void TestMgmtEntryToDeliveryEntry()
        {
            TestMgmtEntry1 mgmt = new TestMgmtEntry1
            {
                Title = new System.Collections.Generic.Dictionary<string, string> { { "en-US", "this is the title" } },
                Number = new System.Collections.Generic.Dictionary<string, int> { { "en-US", 3 } },
                IsTrue = new System.Collections.Generic.Dictionary<string, bool> { { "en-US", true } }
            };
            Entry<TestMgmtEntry1> entry = new Entry<TestMgmtEntry1>();
            entry.Fields = mgmt;
            TestDlvyEntry1 dlvy = entry.ToDeliveryEntry<TestDlvyEntry1, TestMgmtEntry1>("en-US");
            Assert.IsNotNull(dlvy);
            Assert.AreEqual(mgmt.Title["en-US"], dlvy.Title);
            Assert.AreEqual(mgmt.Number["en-US"], dlvy.Number);
            Assert.AreEqual(mgmt.IsTrue["en-US"], dlvy.IsTrue);
        }

        [TestMethod]
        public void TestDeliveryEntryToMgmtEntry()
        {
            TestDlvyEntry1 dlvy = new TestDlvyEntry1
            {
                Title = "this is a title",
                Number = 3,
                IsTrue = true
            };

            Entry<TestMgmtEntry1> mgmt = dlvy.ToManagementEntry<TestMgmtEntry1, TestDlvyEntry1>("en-US");
            Assert.IsNotNull(mgmt);
            Assert.AreEqual(dlvy.Title, mgmt.Fields.Title["en-US"]);
            Assert.AreEqual(dlvy.Number, mgmt.Fields.Number["en-US"]);
            Assert.AreEqual(dlvy.IsTrue, mgmt.Fields.IsTrue["en-US"]);
        }
    }
}
