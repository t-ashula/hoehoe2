// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestUtils.cs">
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
using System.Threading;

namespace Hoehoe3.Core.Tests
{
    internal static class TestUtils
    {
        public static int GetNextRandom(int maxValue = int.MaxValue)
        {
            var r = new Random(Thread.CurrentThread.ManagedThreadId);
            return r.Next(maxValue);
        }

        public static void CreateEmptyFile(string path)
        {
            using (File.Create(path))
            {
            }
        }

        public static void TryDeleteTestFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch
            {
                // ignore
            }
        }
    }
}