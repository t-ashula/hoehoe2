// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserExtenstionAttribute.cs">
// Hoehoe2 - Client of Twitter
// Copyright (c) 2016- t.ashula (@t_ashula) office@ashula.info
// All rights reserved. This file is part of Hoehoe2.
// This program is license under GPL v3 or any later version.
// See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel.Composition;
using System.Linq;

namespace Hoehoe3.Core
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    public class UserExtenstionAttribute : ExportAttribute, IUserExtensionMetadata
    {
        public string ProviderName { get; }

        public UserExtenstionAttribute(string provider)
            : base(typeof(IUserExtension))
        {
            ProviderName = provider;
        }
    }
}