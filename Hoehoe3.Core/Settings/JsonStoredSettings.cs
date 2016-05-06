// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonStoredSettings.cs">
// Hoehoe2 - Client of Twitter
// Copyright (c) 2016- t.ashula (@t_ashula) <office@ashula.info>
// All rights reserved. This file is part of Hoehoe2.
// This program is license under GPL v3 or any later version.
// See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Hoehoe3.Core.Settings.JsonStorage;
using Newtonsoft.Json;

using R = Hoehoe3.Core.Properties.Resources;

namespace Hoehoe3.Core.Settings
{
    /// <summary> SettingsStorage using JSON </summary>
    internal class JsonStoredSettings : IStoredSettings
    {
        public JsonStoredSettings(string locaiton)
        {
            Location = locaiton;
            Load();
            HireGateKeepers();
        }

        /// <inheritdoc />
        public void Load()
        {
            if (string.IsNullOrEmpty(Location))
            {
                _storage = new Storage();
                return;
            }

            var ser = CreateSerializer();
            using (var sr = new StreamReader(Location))
            using (var jr = new JsonTextReader(sr))
            {
                _storage = ser.Deserialize<Storage>(jr);
            }
        }

        /// <inheritdoc />
        public void Store()
        {
            if (string.IsNullOrEmpty(Location))
            {
                // todo: should throw some exception?
                return;
            }

            var ser = CreateSerializer();
            using (var sw = new StreamWriter(Location))
            using (var jw = new JsonTextWriter(sw))
            {
                ser.Serialize(jw, _storage);
            }
        }

        /// <inheritdoc />
        public string this[string key]
        {
            get { return GetStoredValue(key); }
            set { StoreValue(key, value); }
        }

        public string Location { get; set; }

        private Storage _storage;

        private readonly List<string> _unknownSettings = new List<string>();
        private readonly Dictionary<string, SettingsValueGateKeeper<Storage>> _gateKeepers = new Dictionary<string, SettingsValueGateKeeper<Storage>>();

        private static JsonSerializer CreateSerializer()
        {
            return new JsonSerializer();
        }

        private string GetStoredValue(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(string.Format(R.ArgumentExceptionMessageTemplate, nameof(key)), nameof(key));
            }

            SettingsValueGateKeeper<Storage> keeper;
            if (_gateKeepers.TryGetValue(key, out keeper))
            {
                return keeper.GetValue(_storage);
            }

            // todo: logging
            _unknownSettings.Add($"key:{key}");
            return string.Empty;
        }

        private void StoreValue(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(string.Format(R.ArgumentExceptionMessageTemplate, nameof(key)), nameof(key));
            }

            SettingsValueGateKeeper<Storage> keeper;
            if (_gateKeepers.TryGetValue(key, out keeper))
            {
                keeper.SetValue(_storage, value);
            }
            else
            {
                // todo: logging
                _unknownSettings.Add($"key:{key}\tvalue:{value}");
            }
        }

        private void HireGateKeepers()
        {
            const string keyUserName = Settings.KeyUserName;
            _gateKeepers.Add(keyUserName, new SettingsValueGateKeeper<Storage>(keyUserName, (s, k) => s.Accounts[0].UserName, (s, k, v) => s.Accounts[0].UserName = v));
        }
    }
}