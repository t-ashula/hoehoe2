// Hoehoe - Client of Twitter
// Copyright (c) 2011- t.ashula (@t_ashula) <office@ashula.info>
//
// All rights reserved.
// This file is part of Hoehoe.
//
// This program is free software; you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation; either version 3 of the License, or (at your option)
// any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this program. If not, see <http://www.gnu.org/licenses/>, or write to
// the Free Software Foundation, Inc., 51 Franklin Street - Fifth Floor,
// Boston, MA 02110-1301, USA.

using System;
using System.Linq.Expressions;

namespace Hoehoe
{
    public static class DynamicExpression
    {
        public static Expression<Func<T, TS>> ParseLambda<T, TS>(string expression, params object[] values)
        {
            return (Expression<Func<T, TS>>)Dummy();
        }

        private static LambdaExpression Dummy()
        {
            Expression<Func<PostClass, bool>> f = p => true;
            return f;
        }
    }

#if notyet
    public sealed class ParseException : Exception
    {
    }
#endif
}