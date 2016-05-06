// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsTests.cs">
// Hoehoe2 - Client of Twitter
// Copyright (c) 2016- t.ashula (@t_ashula) <office@ashula.info>
// All rights reserved. This file is part of Hoehoe2.
// This program is license under GPL v3 or any later version.
// See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Hoehoe3.Core.Tests
{
    public class SettingsTests
    {
        [Fact]
        public void SerializeTest()
        {
            var ss = new Settings.Settings();
            var storage = $"settings.{TestUtils.GetNextRandom()}.conf";
            ss.Serialize(storage);
            Assert.True(File.Exists(storage));
            TestUtils.TryDeleteTestFile(storage);
        }

        [Fact]
        public void DeserializeTest()
        {
            var storage = $"settings.{TestUtils.GetNextRandom()}.conf";
            {
                var ss = new Settings.Settings();
                ss.Serialize(storage);
                Assert.True(File.Exists(storage));
            }

            var nss = new Settings.Settings();
            nss.Deserialize(storage);

            TestUtils.TryDeleteTestFile(storage);
        }

        [Fact]
        public void LoadTest()
        {
            var s = new Settings.Settings();
            var actual = s.Load("no such key");
            Assert.Equal(string.Empty, actual);
        }

        [Fact]
        public void LoadTest_KnownKey()
        {
            var s = new Settings.Settings();
            var username = $"User.{TestUtils.GetNextRandom()}";
            s.UserName = username;
            var actual = s.Load(Settings.Settings.KeyUserName);
            Assert.Equal(username, actual);
        }

        [Fact]
        public void StoreTest()
        {
            var settings = new Settings.Settings();
            var ua = $"Hoehoe2.{TestUtils.GetNextRandom()}";
            var key = $"no such key.{TestUtils.GetNextRandom()}";
            settings.Store(key, ua);
            Assert.Equal(string.Empty, settings.Load(key));
        }
    }
}