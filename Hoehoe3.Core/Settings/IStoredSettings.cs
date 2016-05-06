// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStoredSettings.cs">
// Hoehoe2 - Client of Twitter
// Copyright (c) 2016- t.ashula (@t_ashula) <office@ashula.info>
// All rights reserved. This file is part of Hoehoe2.
// This program is license under GPL v3 or any later version.
// See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;

namespace Hoehoe3.Core.Settings
{
    internal interface IStoredSettings
    {
        /// <summary> load settings from <see cref="Location"/> </summary>
        void Load();

        /// <summary> store settings to <see cref="Location"/> </summary>
        void Store();

        /// <summary> get or set settings </summary>
        /// <param name="key">key to get or set</param>
        /// <returns>stored setting</returns>
        string this[string key] { get; set; }

        /// <summary>
        /// Location
        /// </summary>
        string Location { get; set; }
    }
}