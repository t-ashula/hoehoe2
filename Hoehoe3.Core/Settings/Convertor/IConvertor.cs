// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IConvertor.cs">
// Hoehoe2 - Client of Twitter
// Copyright (c) 2016- t.ashula (@t_ashula) office@ashula.info
// All rights reserved. This file is part of Hoehoe2.
// This program is license under GPL v3 or any later version.
// See LICENSE.txt
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Hoehoe3.Core.Settings.Convertor
{
    public interface IConvertor
    {
        ISettings ToHoehoeFormat();

        bool FromHoehoeFormat(ISettings settings);
    }
}