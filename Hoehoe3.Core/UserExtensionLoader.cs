// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserExtensionLoader.cs">
// Hoehoe2 - Client of Twitter
// Copyright (c) 2016- t.ashula (@t_ashula) office@ashula.info
// All rights reserved. This file is part of Hoehoe2.
// This program is license under GPL v3 or any later version.
// See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Hoehoe3.Core
{
    internal class UserExtensionLoader
    {
        private Dictionary<string, IUserExtension> _extensions;

        private CompositionContainer _container;

        [ImportMany(AllowRecomposition = true)]
        private IEnumerable<Lazy<IUserExtension, IUserExtensionMetadata>> _parts;

        static UserExtensionLoader()
        {
            Default = new UserExtensionLoader();
        }

        private static readonly Assembly ExecutingAssembly = Assembly.GetExecutingAssembly();

        private static IEnumerable<string> GetPlugInDirectories()
        {
            var codebase = new Uri(ExecutingAssembly.EscapedCodeBase);
            var exePath = codebase.Scheme == "file" ? codebase.LocalPath : codebase.ToString();
            var baseDir = Path.GetDirectoryName(exePath);
            return new[] { baseDir, Path.Combine(baseDir, "plugins") };
        }

        public static UserExtensionLoader Default { get; }

        /// <summary> load extensions </summary>
        /// <returns> whether load extensions success or not </returns>
        public bool LoadExtensions(bool reload = false)
        {
            if (_extensions != null && !reload)
            {
                return true;
            }

            try
            {
                var catalog = new AggregateCatalog();
                catalog.Catalogs.Add(new AssemblyCatalog(typeof(UserExtensionLoader).Assembly));
                foreach (var dir in GetPlugInDirectories().Where(Directory.Exists))
                {
                    catalog.Catalogs.Add(new DirectoryCatalog(dir));
                }

                _container = new CompositionContainer(catalog);
                _container.ComposeParts(this);
            }
            catch (CompositionException e)
            {
                Debug.Print("extension loading error\n\n{0}", e);
                _parts = new Lazy<IUserExtension, IUserExtensionMetadata>[0];
                return false;
            }

            _extensions = new Dictionary<string, IUserExtension>();
            foreach (var p in _parts)
            {
                var loader = p.Value;
                var provider = p.Metadata.ProviderName;
                _extensions.Add(provider, loader);
            }

            return true;
        }

        /// <summary> reload extensions </summary>
        /// <returns> whether reload success or not </returns>
        public bool ReloadExtensions()
        {
            return LoadExtensions(true);
        }

        /// <summary> Gets list of extension provider name </summary>
        public IList<string> Providers => _extensions.Keys.ToList();

        /// <summary> Gets UserExtension that <paramref name="provider"/> provides </summary>
        /// <param name="provider">extension provider name</param>
        /// <returns>IUserExtension</returns>
        public IUserExtension Extension(string provider)
        {
            IUserExtension ue;
            return _extensions.TryGetValue(provider, out ue) ? ue : null;
        }
    }
}