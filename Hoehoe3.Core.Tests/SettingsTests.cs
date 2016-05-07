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
using System.Threading.Tasks;
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
        public void SerializeTest_WithValue()
        {
            var ss = new Settings.Settings
            {
                UserName = $"User.{TestUtils.GetNextRandom()}"
            };
            var storage = $"settings.{TestUtils.GetNextRandom()}.conf";
            ss.Serialize(storage);
            Assert.True(File.Exists(storage));
            TestUtils.TryDeleteTestFile(storage);
        }

        [Fact]
        public void SerializeTest_ToEmptyLocation()
        {
            var ss = new Settings.Settings();
            ss.Serialize(string.Empty);
            // no exception; now
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
        public void DeserializeTest_WithValue()
        {
            var userName = $"User.{TestUtils.GetNextRandom()}";
            var storage = $"settings.{TestUtils.GetNextRandom()}.conf";
            {
                var ss = new Settings.Settings
                {
                    UserName = userName
                };

                ss.Serialize(storage);
                Assert.True(File.Exists(storage));
            }

            var nss = new Settings.Settings();
            nss.Deserialize(storage);
            Assert.Equal(userName, nss.UserName);

            TestUtils.TryDeleteTestFile(storage);
        }

        [Fact]
        public void DeserializeTest_FromEmptyLocation()
        {
            var s = new Settings.Settings();
            s.Deserialize(string.Empty);
            // no exception; now
        }

        [Fact]
        public void LoadTest()
        {
            var s = new Settings.Settings();
            var actual = s.Load("no such key");
            Assert.Null(actual);
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
        public async void LoadTest_EmptyKey()
        {
            var s = new Settings.Settings();
            await Assert.ThrowsAsync<ArgumentException>(() => Task.Run(() => s.Load(string.Empty)));
        }

        [Fact]
        public void StoreTest()
        {
            var s = new Settings.Settings();
            var ua = $"Hoehoe2.{TestUtils.GetNextRandom()}";
            var key = $"no such key.{TestUtils.GetNextRandom()}";
            s.Store(key, ua);
            Assert.Null(s.Load(key));
        }

        [Fact]
        public void StoreTest_KnownKey()
        {
            var s = new Settings.Settings();
            var username = $"User.{TestUtils.GetNextRandom()}";
            s.Store(Settings.Settings.KeyUserName, username);
            Assert.Equal(username, s.Load(Settings.Settings.KeyUserName));
        }
    }
}