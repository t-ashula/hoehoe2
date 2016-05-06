// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsValueGateKeeper.cs">
// Hoehoe2 - Client of Twitter
// Copyright (c) 2016- t.ashula (@t_ashula) <office@ashula.info>
// All rights reserved. This file is part of Hoehoe2.
// This program is license under GPL v3 or any later version.
// See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;

namespace Hoehoe3.Core.Settings.JsonStorage
{
    internal class SettingsValueGateKeeper<T>
    {
        private readonly string _key;
        private readonly Func<T, string, string> _getter;
        private readonly Action<T, string, string> _setter;

        public SettingsValueGateKeeper(string key, Func<T, string, string> getter, Action<T, string, string> setter)
        {
            _key = key;
            _getter = getter;
            _setter = setter;
        }

        public string GetValue(T settings)
        {
            return _getter(settings, _key);
        }

        public void SetValue(T settings, string value)
        {
            _setter(settings, _key, value);
        }
    }
}