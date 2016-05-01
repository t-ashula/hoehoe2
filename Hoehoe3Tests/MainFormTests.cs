using System;
using System.Linq;
using NUnit.Framework;

namespace Hoehoe3.Tests
{
    [TestFixture]
    public class MainFormTests
    {
        [Test]
        public void MainFormTest()
        {
            Assert.Inconclusive();
        }

        [Test]
        public void MainFormHasMenuTest()
        {
            var f = new MainForm();
            var m = f.Menu;
            Assert.IsNotNull(m, "MainForm has Menu");
        }

        //[Test]
        //public void IsSamePathTest()
        //{
        //    var result = Program.IsSameFilePath(@"C:/", @"c:/");
        //    Assert.IsTrue(result);
        //}
    }
}