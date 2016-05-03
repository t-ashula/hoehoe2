// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CollectionExtensionsTests.cs">
//   bl4n - Backlog.jp API Client library
//   this file is part of bl4n, license under MIT license. http://t-ashula.mit-license.org/2015
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using Xunit;

namespace Hoehoe3.Tests
{
    public class MainFormTests
    {
        [Fact]
        public void MainFormHasMenuTest()
        {
            var f = new MainForm();
            var m = f.Menu;
            Assert.NotNull(m);
        }
    }
}