// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Storage.cs">
// Hoehoe2 - Client of Twitter
// Copyright (c) 2016- t.ashula (@t_ashula) office@ashula.info
// All rights reserved. This file is part of Hoehoe2.
// This program is license under GPL v3 or any later version.
// See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Runtime.Serialization;

namespace Hoehoe3.Core.Settings.JsonStorage
{
    /// <summary> class for serialize settings </summary>
    [DataContract]
    internal class Storage
    {
        /// <summary> Initializes a new instance of the <see cref="Storage"/> class. </summary>
        public Storage()
        {
            Accounts = new[] { new Account() };
        }

        [DataMember]
        public Account[] Accounts { get; set; }
    }
}