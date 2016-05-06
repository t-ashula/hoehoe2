// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Account.cs">
// Hoehoe2 - Client of Twitter
// Copyright (c) 2016- t.ashula (@t_ashula) <office@ashula.info>
// All rights reserved. This file is part of Hoehoe2.
// This program is license under GPL v3 or any later version.
// See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Runtime.Serialization;

namespace Hoehoe3.Core.Settings
{
    [DataContract]
    public class Account
    {
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public ServiceTypes ServiceType { get; set; }

        public enum ServiceTypes
        {
            Twitter,
        }
    }
}