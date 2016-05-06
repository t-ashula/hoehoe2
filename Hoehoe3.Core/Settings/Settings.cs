// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Settings.cs">
// Hoehoe2 - Client of Twitter
// Copyright (c) 2016- t.ashula (@t_ashula) <office@ashula.info>
// All rights reserved. This file is part of Hoehoe2.
// This program is license under GPL v3 or any later version.
// See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Linq;

namespace Hoehoe3.Core.Settings
{
    public partial class Settings : ISettings, INotifyPropertyChanged
    {
        public Settings(string location = "")
        {
            Deserialize(location);
        }

        public string Load(string key)
        {
            return _storage[key];
        }

        public void Store(string key, string value)
        {
            if (_storage[key] != value)
            {
                _storage[key] = value;
                OnPropertyChanged(key);
            }
        }

        public void Deserialize(string location)
        {
            _storage = new JsonStoredSettings(location);
            _storage.Load();
        }

        public void Serialize(string location)
        {
            _storage.Location = location;
            _storage.Store();
        }

        private IStoredSettings _storage;

        public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}