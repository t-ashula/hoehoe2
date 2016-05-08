// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonStoredSettingsTests.cs">
// Hoehoe2 - Client of Twitter
// Copyright (c) 2016- t.ashula (@t_ashula) office@ashula.info
// All rights reserved. This file is part of Hoehoe2.
// This program is license under GPL v3 or any later version.
// See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Hoehoe3.Core.Settings;
using Xunit;

namespace Hoehoe3.Core.Tests
{
    public class JsonStoredSettingsTests
    {
        [Fact]
        public void ConstructorTest()
        {
            var jss = new JsonStoredSettings(string.Empty);
            Assert.NotNull(jss);
        }

        [Fact]
        public async void ConstructorTest_NoFile()
        {
            var location = $"jss.{TestUtils.GetNextRandom()}.config";
            await Assert.ThrowsAsync<FileNotFoundException>(() => Task.Run(() => new JsonStoredSettings(location)));
        }

        [Fact]
        public void ConstructorTest_WithEmptyFile()
        {
            var path = $"jss.{TestUtils.GetNextRandom()}.config";

            TestUtils.CreateEmptyFile(path);
            Assert.True(File.Exists(path));

            var jss = new JsonStoredSettings(path);
            Assert.NotNull(jss);
            Assert.Equal(path, jss.Location);
            TestUtils.TryDeleteTestFile(path);
        }

        [Fact]
        public void StoreTest()
        {
            var path = $"jss.{TestUtils.GetNextRandom()}.config";
            var jss = new JsonStoredSettings(string.Empty)
            {
                Location = path
            };

            jss.Store();
            Assert.True(File.Exists(path));

            TestUtils.TryDeleteTestFile(path);
        }

        [Fact]
        public void StoreTest_WithEmptyLocation()
        {
            var jss = new JsonStoredSettings(string.Empty);
            jss.Store();
            //// no exception;
        }

        [Fact]
        public void IndexerGetterTest()
        {
            var jss = new JsonStoredSettings(string.Empty);
            var val = jss["no such key"];
            Assert.Null(val);
        }

        [Fact]
        public async void IndexerGetterTest_EmptyKey()
        {
            var jss = new JsonStoredSettings(string.Empty);
            await Assert.ThrowsAsync<ArgumentException>(() => Task.Run(() => jss[string.Empty]));
        }

        [Fact]
        public void IndexerSetterTest()
        {
            var user = $"User.{TestUtils.GetNextRandom()}";
            var jss = new JsonStoredSettings(string.Empty);
            jss[Settings.Settings.KeyUserName] = user;
            Assert.Equal(user, jss[Settings.Settings.KeyUserName]);
        }

        [Fact]
        public async void IndexerSetterTest_EmptyKey()
        {
            var jss = new JsonStoredSettings(string.Empty);

            await Assert.ThrowsAsync<ArgumentException>(() => Task.Run(() => jss[string.Empty] = string.Empty));
        }

        [Fact]
        public void IndexerSetterTest_UnknownKey()
        {
            var jss = new JsonStoredSettings(string.Empty);
            jss["NoSuchKey"] = string.Empty;
            Assert.Null(jss["NoSuchKey"]);
        }
    }
}