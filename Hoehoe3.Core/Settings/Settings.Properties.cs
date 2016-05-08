// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Settings.Properties.cs">
// Hoehoe2 - Client of Twitter
// Copyright (c) 2016- t.ashula (@t_ashula) office@ashula.info
// All rights reserved. This file is part of Hoehoe2.
// This program is license under GPL v3 or any later version.
// See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;

namespace Hoehoe3.Core.Settings
{
    public partial class Settings
    {
        /// <summary> The key user name. </summary>
        public const string KeyUserName = "UserName";

        /// <summary>
        /// Gets or sets the user name.
        /// </summary>
        public string UserName
        {
            get { return _storage[KeyUserName]; }
            set { _storage[KeyUserName] = value; }
        }
    }
}