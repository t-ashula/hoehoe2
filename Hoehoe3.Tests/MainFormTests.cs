// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainFromTests.cs">
// Hoehoe - Client of Twitter
// Copyright (c) 2016- t.ashula (@t_ashula) <office@ashula.info>
// All rights reserved. This file is part of Hoehoe.
// This program is license under GPL v3 or any later version.
// See LICENSE.txt
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