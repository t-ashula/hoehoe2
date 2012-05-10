using System;
using System.Collections;
using System.Collections.Generic;

// Tween - Client of Twitter
// Copyright (c) 2007-2011 kiri_feather (@kiri_feather) <kiri.feather@gmail.com>
//           (c) 2008-2011 Moz (@syo68k)
//           (c) 2008-2011 takeshik (@takeshik) <http://www.takeshik.org/>
//           (c) 2010-2011 anis774 (@anis774) <http://d.hatena.ne.jp/anis774/>
//           (c) 2010-2011 fantasticswallow (@f_swallow) <http://twitter.com/f_swallow>
// All rights reserved.
//
// This file is part of Tween.
//
// This program is free software;
//
// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//

using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace Tween
{
    public static class DynamicQueryable
    {
        const string Id = "DynamicQueryModule Copyright © Microsoft Corporation.  All Rights Reserved." + " This code released under the terms of the Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.) ";

        [Extension()]
        public static IQueryable<T> Where<T>(IQueryable<T> source, string predicate, params object[] values)
        {
            return (IQueryable<T>)Where((IQueryable)source, predicate, values);
        }

        [Extension()]
        public static IQueryable Where(IQueryable source, string predicate, params object[] values)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (predicate == null)
                throw new ArgumentNullException("predicate");
            LambdaExpression lambda = DynamicExpression.ParseLambda(source.ElementType, typeof(bool), predicate, values);
            return source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "Where", new Type[] { source.ElementType }, source.Expression, Expression.Quote(lambda)));
        }

        [Extension()]
        public static IQueryable Select(IQueryable source, string selector, params object[] values)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (selector == null)
                throw new ArgumentNullException("selector");
            LambdaExpression lambda = DynamicExpression.ParseLambda(source.ElementType, null, selector, values);
            return source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "Select", new Type[] {
				source.ElementType,
				lambda.Body.Type
			}, source.Expression, Expression.Quote(lambda)));
        }

        [Extension()]
        public static IQueryable<T> OrderBy<T>(IQueryable<T> source, string ordering, params object[] values)
        {
            return (IQueryable<T>)OrderBy((IQueryable)source, ordering, values);
        }

        [Extension()]
        public static IQueryable OrderBy(IQueryable source, string ordering, params object[] values)
        {
            if ((source == null))
                throw new ArgumentNullException("source");
            if ((ordering == null))
                throw new ArgumentNullException("ordering");
            var parameters = new ParameterExpression[] { Expression.Parameter(source.ElementType, "") };
            ExpressionParser parser = new ExpressionParser(parameters, ordering, values);
            IEnumerable<DynamicOrdering> orderings = parser.ParseOrdering();
            Expression queryExpr = source.Expression;
            var methodAsc = "OrderBy";
            var methodDesc = "OrderByDescending";
            foreach (DynamicOrdering o in orderings)
            {
                queryExpr = Expression.Call(typeof(Queryable), o.Ascending ? methodAsc : methodDesc, new Type[] {
					source.ElementType,
					o.Selector.Type
				}, queryExpr, Expression.Quote(Expression.Lambda(o.Selector, parameters)));
                methodAsc = "ThenBy";
                methodDesc = "ThenByDescending";
            }
            return source.Provider.CreateQuery(queryExpr);
        }

        [Extension()]
        public static IQueryable Take(IQueryable source, int count)
        {
            if ((source == null))
                throw new ArgumentNullException("source");
            return source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "Take", new Type[] { source.ElementType }, source.Expression, Expression.Constant(count)));
        }

        [Extension()]
        public static IQueryable Skip(IQueryable source, int count)
        {
            if ((source == null))
                throw new ArgumentNullException("source");
            return source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "Skip", new Type[] { source.ElementType }, source.Expression, Expression.Constant(count)));
        }

        [Extension()]
        public static IQueryable GroupBy(IQueryable source, string keySelector, string elementSelector, params object[] values)
        {
            if ((source == null))
                throw new ArgumentNullException("source");
            if ((keySelector == null))
                throw new ArgumentNullException("keySelector");
            if ((elementSelector == null))
                throw new ArgumentNullException("elementSelector");
            LambdaExpression keyLambda = DynamicExpression.ParseLambda(source.ElementType, null, keySelector, values);
            LambdaExpression elementLambda = DynamicExpression.ParseLambda(source.ElementType, null, elementSelector, values);
            return source.Provider.CreateQuery(Expression.Call(typeof(Queryable), "GroupBy", new Type[] {
				source.ElementType,
				keyLambda.Body.Type,
				elementLambda.Body.Type
			}, source.Expression, Expression.Quote(keyLambda), Expression.Quote(elementLambda)));
        }

        [Extension()]
        public static bool Any(IQueryable source)
        {
            if ((source == null))
                throw new ArgumentNullException("source");
            return Convert.ToBoolean(source.Provider.Execute(Expression.Call(typeof(Queryable), "Any", new Type[] { source.ElementType }, source.Expression)));
        }

        [Extension()]
        public static int Count(IQueryable source)
        {
            if ((source == null))
                throw new ArgumentNullException("source");
            return Convert.ToInt32(source.Provider.Execute(Expression.Call(typeof(Queryable), "Count", new Type[] { source.ElementType }, source.Expression)));
        }
    }
}

namespace Tween
{
    public abstract class DynamicClass
    {
        public override string ToString()
        {
            var props = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            for (int i = 0; i <= props.Length - 1; i++)
            {
                if ((i > 0))
                    sb.Append(", ");
                sb.Append(props[i].Name);
                sb.Append("=");
                sb.Append(props[i].GetValue(this, null));
            }

            sb.Append("}");

            return sb.ToString();
        }
    }
}

namespace Tween
{
    public class DynamicProperty
    {
        private string _name;

        private Type _type;

        public DynamicProperty(string name, Type type)
        {
            if ((name == null))
                throw new ArgumentNullException("name");
            if ((type == null))
                throw new ArgumentNullException("type");
            this._name = name;
            this._type = type;
        }

        public string Name
        {
            get { return _name; }
        }

        public Type Type
        {
            get { return _type; }
        }
    }
}

namespace Tween
{
    public static class DynamicExpression
    {
        public static Expression Parse(Type resultType, string expression, params object[] values)
        {
            ExpressionParser parser = new ExpressionParser(null, expression, values);
            return parser.Parse(resultType);
        }

        public static LambdaExpression ParseLambda(Type itType, Type resultType, string expressionStr, params object[] values)
        {
            return ParseLambda(new ParameterExpression[] { Expression.Parameter(itType, "") }, resultType, expressionStr, values);
        }

        public static LambdaExpression ParseLambda(ParameterExpression[] parameters, Type resultType, string expressionStr, params object[] values)
        {
            ExpressionParser parser = new ExpressionParser(parameters, expressionStr, values);
            return Expression.Lambda(parser.Parse(resultType), parameters);
        }

        public static Expression<Func<T, S>> ParseLambda<T, S>(string expression, params object[] values)
        {
            return (Expression<Func<T, S>>)ParseLambda(typeof(T), typeof(S), expression, values);
        }

        public static Type CreateClass(params DynamicProperty[] properties)
        {
            return ClassFactory.Instance.GetDynamicClass(properties);
        }

        public static Type CreateClass(IEnumerable<DynamicProperty> properties)
        {
            return ClassFactory.Instance.GetDynamicClass(properties);
        }
    }
}

namespace Tween
{
    internal class DynamicOrdering
    {
        public Expression Selector;
        public bool Ascending;
    }
}

namespace Tween
{
    internal class Signature : IEquatable<Signature>
    {
        public DynamicProperty[] properties;

        public int hashCode;

        public Signature(IEnumerable<DynamicProperty> properties)
        {
            this.properties = properties.ToArray();
            hashCode = 0;
            foreach (DynamicProperty p in this.properties)
            {
                hashCode = hashCode ^ p.Name.GetHashCode() ^ p.Type.GetHashCode();
            }
        }

        public override int GetHashCode()
        {
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            var cast = obj as Signature;
            return cast != null ? Equals(cast) : false;
        }

        public bool Equals(Signature other)
        {
            if ((properties.Length != other.properties.Length))
                return false;
            for (int i = 0; i <= properties.Length - 1; i++)
            {
                if ((properties[i].Name != other.properties[i].Name || !properties[i].Type.Equals(other.properties[i].Type)))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

namespace Tween
{
    internal class ClassFactory
    {
        public static readonly ClassFactory Instance = new ClassFactory();

        static ClassFactory()
        {
            // Trigger lazy initialization of static fields
        }

        private ModuleBuilder module;
        private Dictionary<Signature, Type> classes;
        private int classCount;

        private ReaderWriterLock rwLock;

        private ClassFactory()
        {
            AssemblyName name = new AssemblyName("DynamicClasses");
            AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
#if ENABLE_LINQ_PARTIAL_TRUST
			new ReflectionPermission(PermissionState.Unrestricted).Assert();
#endif
            try
            {
                module = assembly.DefineDynamicModule("Module");
            }
            finally
            {
#if ENABLE_LINQ_PARTIAL_TRUST
				PermissionSet.RevertAssert();
#endif
            }
            classes = new Dictionary<Signature, Type>();
            rwLock = new ReaderWriterLock();
        }

        public Type GetDynamicClass(IEnumerable<DynamicProperty> properties)
        {
            rwLock.AcquireReaderLock(Timeout.Infinite);

            try
            {
                Signature signature = new Signature(properties);
                Type type = null;
                if (!classes.TryGetValue(signature, out type))
                {
                    type = CreateDynamicClass(signature.properties);
                    classes.Add(signature, type);
                }
                return type;
            }
            finally
            {
                rwLock.ReleaseReaderLock();
            }
        }

        private Type CreateDynamicClass(DynamicProperty[] properties)
        {
            LockCookie cookie = rwLock.UpgradeToWriterLock(Timeout.Infinite);
            try
            {
                var typeName = "DynamicClass" + (classCount + 1);
#if ENABLE_LINQ_PARTIAL_TRUST
				new ReflectionPermission(PermissionState.Unrestricted).Assert();
#endif
                try
                {
                    TypeBuilder tb = this.module.DefineType(typeName, TypeAttributes.Class | TypeAttributes.Public, typeof(DynamicClass));
                    FieldInfo[] fields = GenerateProperties(tb, properties);
                    GenerateEquals(tb, fields);
                    GenerateGetHashCode(tb, fields);
                    Type result = tb.CreateType();
                    classCount += 1;
                    return result;
                }
                finally
                {
#if ENABLE_LINQ_PARTIAL_TRUST
					PermissionSet.RevertAssert();
#endif
                }
            }
            finally
            {
                rwLock.DowngradeFromWriterLock(ref cookie);
            }
        }

        private FieldInfo[] GenerateProperties(TypeBuilder tb, DynamicProperty[] properties)
        {
            FieldInfo[] fields = new FieldInfo[properties.Length];

            for (int i = 0; i <= properties.Length - 1; i++)
            {
                DynamicProperty dp = properties[i];
                FieldBuilder fb = tb.DefineField("_" + dp.Name, dp.Type, FieldAttributes.Private);
                PropertyBuilder pb = tb.DefineProperty(dp.Name, PropertyAttributes.HasDefault, dp.Type, null);
                MethodBuilder mbGet = tb.DefineMethod("get_" + dp.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, dp.Type, System.Type.EmptyTypes);
                ILGenerator genGet = mbGet.GetILGenerator();
                genGet.Emit(OpCodes.Ldarg_0);
                genGet.Emit(OpCodes.Ldfld, fb);
                genGet.Emit(OpCodes.Ret);
                MethodBuilder mbSet = tb.DefineMethod("set_" + dp.Name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new Type[] { dp.Type });
                ILGenerator genSet = mbSet.GetILGenerator();
                genSet.Emit(OpCodes.Ldarg_0);
                genSet.Emit(OpCodes.Ldarg_1);
                genSet.Emit(OpCodes.Stfld, fb);
                genSet.Emit(OpCodes.Ret);
                pb.SetGetMethod(mbGet);
                pb.SetSetMethod(mbSet);
                fields[i] = fb;
            }

            return fields;
        }

        private void GenerateEquals(TypeBuilder tb, FieldInfo[] fields)
        {
            MethodBuilder mb = tb.DefineMethod("Equals", MethodAttributes.Public | MethodAttributes.ReuseSlot | MethodAttributes.Virtual | MethodAttributes.HideBySig, typeof(bool), new Type[] { typeof(object) });
            ILGenerator gen = mb.GetILGenerator();
            LocalBuilder other = gen.DeclareLocal(tb);
            Label next = gen.DefineLabel();
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Isinst, tb);
            gen.Emit(OpCodes.Stloc, other);
            gen.Emit(OpCodes.Ldloc, other);
            gen.Emit(OpCodes.Brtrue_S, next);
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Ret);
            gen.MarkLabel(next);
            foreach (FieldInfo field in fields)
            {
                Type ft = field.FieldType;
                Type ct = typeof(EqualityComparer<object>).GetGenericTypeDefinition().MakeGenericType(ft);
                next = gen.DefineLabel();
                gen.EmitCall(OpCodes.Call, ct.GetMethod("get_Default"), null);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, field);
                gen.Emit(OpCodes.Ldloc, other);
                gen.Emit(OpCodes.Ldfld, field);
                gen.EmitCall(OpCodes.Callvirt, ct.GetMethod("Equals", new Type[] {
					ft,
					ft
				}), null);
                gen.Emit(OpCodes.Brtrue_S, next);
                gen.Emit(OpCodes.Ldc_I4_0);
                gen.Emit(OpCodes.Ret);
                gen.MarkLabel(next);
            }
            gen.Emit(OpCodes.Ldc_I4_1);
            gen.Emit(OpCodes.Ret);
        }

        private void GenerateGetHashCode(TypeBuilder tb, FieldInfo[] fields)
        {
            MethodBuilder mb = tb.DefineMethod("GetHashCode", MethodAttributes.Public | MethodAttributes.ReuseSlot | MethodAttributes.Virtual | MethodAttributes.HideBySig, typeof(int), System.Type.EmptyTypes);
            ILGenerator gen = mb.GetILGenerator();
            gen.Emit(OpCodes.Ldc_I4_0);
            foreach (FieldInfo field in fields)
            {
                Type ft = field.FieldType;
                Type ct = typeof(EqualityComparer<object>).GetGenericTypeDefinition().MakeGenericType(ft);
                gen.EmitCall(OpCodes.Call, ct.GetMethod("get_Default"), null);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, field);
                gen.EmitCall(OpCodes.Callvirt, ct.GetMethod("GetHashCode", new Type[] { ft }), null);
                gen.Emit(OpCodes.Xor);
            }
            gen.Emit(OpCodes.Ret);
        }
    }
}

namespace Tween
{
    public sealed class ParseException : Exception
    {
        private int positionValue;

        public ParseException(string message, int position)
            : base(message)
        {
            this.positionValue = position;
        }

        public int Position
        {
            get { return positionValue; }
        }

        public override string ToString()
        {
            return string.Format(Res.ParseExceptionFormat, Message, Position);
        }
    }
}

namespace Tween
{
    internal class ExpressionParser
    {
        public struct Token
        {
            public TokenId id;
            public string text;
            public int pos;
        }

        public enum TokenId
        {
            Unknown,
            End,
            Identifier,
            StringLiteral,
            IntegerLiteral,
            RealLiteral,
            Exclamation,
            Percent,
            Amphersand,
            OpenParen,
            CloseParen,
            Asterisk,
            Plus,
            Comma,
            Minus,
            Dot,
            Slash,
            Colon,
            LessThan,
            Equal,
            GreaterThan,
            Question,
            OpenBracket,
            CloseBracket,
            Bar,
            ExclamationEqual,
            DoubleAmphersand,
            LessThanEqual,
            LessGreater,
            DoubleEqual,
            GreaterThanEqual,
            DoubleBar
        }

        public interface ILogicalSignatures
        {
            void F(bool x, bool y);

            void F(bool? x, bool? y);
        }

        public interface IArithmeticSignatures
        {
            void F(int x, int y);

            void F(uint x, uint y);

            void F(long x, long y);

            void F(ulong x, ulong y);

            void F(float x, float y);

            void F(double x, double y);

            void F(decimal x, decimal y);

            void F(int? x, int? y);

            void F(uint? x, uint? y);

            void F(long? x, long? y);

            void F(ulong? x, ulong? y);

            void F(float? x, float? y);

            void F(double? x, double? y);

            void F(decimal? x, decimal? y);
        }

        public interface IRelationalSignatures : IArithmeticSignatures
        {
            void F(string x, string y);

            void F(char x, char y);

            void F(DateTime x, DateTime y);

            void F(TimeSpan x, TimeSpan y);

            void F(char? x, char? y);

            void F(Nullable<DateTime> x, Nullable<DateTime> y);

            void F(Nullable<TimeSpan> x, Nullable<TimeSpan> y);
        }

        public interface IEqualitySignatures : IRelationalSignatures
        {
            void F(bool x, bool y);

            void F(bool? x, bool? y);
        }

        public interface IAddSignatures : IArithmeticSignatures
        {
            void F(DateTime x, TimeSpan y);

            void F(TimeSpan x, TimeSpan y);

            void F(Nullable<DateTime> x, Nullable<TimeSpan> y);

            void F(Nullable<TimeSpan> x, Nullable<TimeSpan> y);
        }

        public interface ISubtractSignatures : IAddSignatures
        {
            void F(DateTime x, DateTime y);

            void F(Nullable<DateTime> x, Nullable<DateTime> y);
        }

        public interface INegationSignatures
        {
            void F(int x);

            void F(long x);

            void F(float x);

            void F(double x);

            void F(decimal x);

            void F(int? x);

            void F(long? x);

            void F(float? x);

            void F(double? x);

            void F(decimal? x);
        }

        public interface INotSignatures
        {
            void F(bool x);

            void F(bool? x);
        }

        public interface IEnumerableSignatures
        {
            void Where(bool predicate);

            void Any();

            void Any(bool predicate);

            void All(bool predicate);

            void Count();

            void Count(bool predicate);

            void Min(object selector);

            void Max(object selector);

            void Sum(int selector);

            void Sum(int? selector);

            void Sum(long selector);

            void Sum(long? selector);

            void Sum(float selector);

            void Sum(float? selector);

            void Sum(double selector);

            void Sum(double? selector);

            void Sum(decimal selector);

            void Sum(decimal? selector);

            void Average(int selector);

            void Average(int? selector);

            void Average(long selector);

            void Average(long? selector);

            void Average(float selector);

            void Average(float? selector);

            void Average(double selector);

            void Average(double? selector);

            void Average(decimal selector);

            void Average(decimal? selector);
        }

        static readonly Type[] predefinedTypes = {
			typeof(object),
			typeof(bool),
			typeof(char),
			typeof(string),
			typeof(sbyte),
			typeof(byte),
			typeof(Int16),
			typeof(UInt16),
			typeof(Int32),
			typeof(UInt32),
			typeof(Int64),
			typeof(UInt64),
			typeof(float),
			typeof(double),
			typeof(decimal),
			typeof(DateTime),
			typeof(TimeSpan),
			typeof(Guid),
			typeof(Math),
			typeof(Convert),
			typeof(Regex)
		};
        static readonly Expression trueLiteral = Expression.Constant(true);
        static readonly Expression falseLiteral = Expression.Constant(false);

        static readonly Expression nullLiteral = Expression.Constant(null);
        static readonly string keywordIt = "it";
        static readonly string keywordIif = "iif";

        static readonly string keywordNew = "new";

        static Dictionary<string, object> keywords;
        Dictionary<string, object> symbols;
        IDictionary<string, object> externals;
        Dictionary<Expression, string> literals;
        ParameterExpression it;
        string text;
        int textPos;
        int textLen;
        char ch;

        Token tokenVal;

        public ExpressionParser(ParameterExpression[] parameters, string expression, object[] values)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");
            if (keywords == null)
                keywords = CreateKeywords();
            symbols = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            literals = new Dictionary<Expression, string>();
            if (parameters != null)
                ProcessParameters(parameters);
            if (values != null)
                ProcessValues(values);
            text = expression;
            textLen = text.Length;
            SetTextPos(0);
            NextToken();
        }

        public void ProcessParameters(ParameterExpression[] parameters)
        {
            foreach (ParameterExpression pe in parameters)
            {
                if (!string.IsNullOrEmpty(pe.Name))
                {
                    AddSymbol(pe.Name, pe);
                }
            }

            if ((parameters.Length == 1 && string.IsNullOrEmpty(parameters[0].Name)))
            {
                it = parameters[0];
            }
        }

        public void ProcessValues(object[] values)
        {
            for (int i = 0; i <= values.Length - 1; i++)
            {
                object value = values[i];
                if (i == values.Length - 1 && value as IDictionary<string, object> != null)
                {
                    externals = (IDictionary<string, object>)value;
                }
                else
                {
                    AddSymbol("@" + i.ToString(System.Globalization.CultureInfo.InvariantCulture), value);
                }
            }
        }

        public void AddSymbol(string name, object value)
        {
            if ((symbols.ContainsKey(name)))
            {
                throw ParseError(Res.DuplicateIdentifier, name);
            }
            symbols.Add(name, value);
        }

        public Expression Parse(Type resultType)
        {
            int exprPos = tokenVal.pos;
            Expression expr = ParseExpression();
            if (resultType != null)
            {
                expr = PromoteExpression(expr, resultType, true);
                if (expr == null)
                {
                    throw ParseError(exprPos, Res.ExpressionTypeMismatch, GetTypeName(resultType));
                }
            }
            ValidateToken(TokenId.End, Res.SyntaxError);
            return expr;
        }

        public IEnumerable<DynamicOrdering> ParseOrdering()
        {
            List<DynamicOrdering> orderings = new List<DynamicOrdering>();
            do
            {
                Expression expr = ParseExpression();
                bool @ascending = true;
                if (TokenIdentifierIs("asc") || TokenIdentifierIs("ascending"))
                {
                    NextToken();
                }
                else if (TokenIdentifierIs("desc") || TokenIdentifierIs("descending"))
                {
                    NextToken();
                    @ascending = false;
                }
                orderings.Add(new DynamicOrdering
                {
                    Selector = expr,
                    Ascending = @ascending
                });
                if (tokenVal.id != TokenId.Comma)
                    break; // TODO: might not be correct. Was : Exit Do
                NextToken();
            } while (true);
            ValidateToken(TokenId.End, Res.SyntaxError);
            return orderings;
        }

        //#pragma warning restore 0219

        // ?: operator
        public Expression ParseExpression()
        {
            int errorPos = tokenVal.pos;
            Expression expr = ParseLogicalOr();
            if (tokenVal.id == TokenId.Question)
            {
                NextToken();
                Expression expr1 = ParseExpression();
                ValidateToken(TokenId.Colon, Res.ColonExpected);
                NextToken();
                Expression expr2 = ParseExpression();
                expr = GenerateConditional(expr, expr1, expr2, errorPos);
            }
            return expr;
        }

        // ||, or,orelse operator
        public Expression ParseLogicalOr()
        {
            Expression left = ParseLogicalAnd();
            while (tokenVal.id == TokenId.DoubleBar || TokenIdentifierIs("or") || TokenIdentifierIs("orelse"))
            {
                Token op = tokenVal;
                NextToken();
                Expression right = ParseLogicalAnd();
                CheckAndPromoteOperands(typeof(ILogicalSignatures), op.text, ref left, ref right, op.pos);
                left = Expression.OrElse(left, right);
            }
            return left;
        }

        // &&, and, andalso operator
        public Expression ParseLogicalAnd()
        {
            Expression left = ParseComparison();
            while (tokenVal.id == TokenId.DoubleAmphersand || TokenIdentifierIs("and") || TokenIdentifierIs("andalso"))
            {
                Token op = tokenVal;
                NextToken();
                Expression right = ParseComparison();
                CheckAndPromoteOperands(typeof(ILogicalSignatures), op.text, ref left, ref right, op.pos);
                left = Expression.AndAlso(left, right);
            }
            return left;
        }

        // =, ==, !=, <>, >, >=, <, <= operators
        public Expression ParseComparison()
        {
            Expression left = ParseAdditive();
            while (tokenVal.id == TokenId.Equal || tokenVal.id == TokenId.DoubleEqual || tokenVal.id == TokenId.ExclamationEqual || tokenVal.id == TokenId.LessGreater || tokenVal.id == TokenId.GreaterThan || tokenVal.id == TokenId.GreaterThanEqual || tokenVal.id == TokenId.LessThan || tokenVal.id == TokenId.LessThanEqual)
            {
                Token op = tokenVal;
                NextToken();
                Expression right = ParseAdditive();
                bool isEquality = (op.id == TokenId.Equal || op.id == TokenId.DoubleEqual || op.id == TokenId.ExclamationEqual || op.id == TokenId.LessGreater);
                if (isEquality && !left.Type.IsValueType && !right.Type.IsValueType)
                {
                    if (!left.Type.Equals(right.Type))
                    {
                        if (left.Type.IsAssignableFrom(right.Type))
                        {
                            right = Expression.Convert(right, left.Type);
                        }
                        else if (right.Type.IsAssignableFrom(left.Type))
                        {
                            left = Expression.Convert(left, right.Type);
                        }
                        else
                        {
                            throw IncompatibleOperandsError(op.text, left, right, op.pos);
                        }
                    }
                }
                else if (IsEnumType(left.Type) || IsEnumType(right.Type))
                {
                    if (!left.Type.Equals(right.Type))
                    {
                        Expression e = PromoteExpression(right, left.Type, true);
                        if (e != null)
                        {
                            right = e;
                        }
                        else
                        {
                            e = PromoteExpression(left, right.Type, true);
                            if (e == null)
                            {
                                throw IncompatibleOperandsError(op.text, left, right, op.pos);
                            }
                            left = e;
                        }
                    }
                }
                else
                {
                    CheckAndPromoteOperands(isEquality ? typeof(IEqualitySignatures) : typeof(IRelationalSignatures), op.text, ref left, ref right, op.pos);
                }
                switch (op.id)
                {
                    case TokenId.Equal:
                    case TokenId.DoubleEqual:
                        left = GenerateEqual(left, right);
                        break;
                    case TokenId.ExclamationEqual:
                    case TokenId.LessGreater:
                        left = GenerateNotEqual(left, right);
                        break;
                    case TokenId.GreaterThan:
                        left = GenerateGreaterThan(left, right);
                        break;
                    case TokenId.GreaterThanEqual:
                        left = GenerateGreaterThanEqual(left, right);
                        break;
                    case TokenId.LessThan:
                        left = GenerateLessThan(left, right);
                        break;
                    case TokenId.LessThanEqual:
                        left = GenerateLessThanEqual(left, right);
                        break;
                }
            }
            return left;
        }

        // +, -, & operators
        public Expression ParseAdditive()
        {
            var left = ParseMultiplicative();
            while (tokenVal.id == TokenId.Plus || tokenVal.id == TokenId.Minus || tokenVal.id == TokenId.Amphersand)
            {
                var op = tokenVal;
                NextToken();
                var right = ParseMultiplicative();
                switch (op.id)
                {
                    case TokenId.Plus:
                        if (left.Type.Equals(typeof(string)) || right.Type.Equals(typeof(string)))
                        {
                            goto amphersand;
                        }
                        CheckAndPromoteOperands(typeof(IAddSignatures), op.text, ref left, ref right, op.pos);
                        left = GenerateAdd(left, right);
                        break;
                    case TokenId.Minus:
                        CheckAndPromoteOperands(typeof(ISubtractSignatures), op.text, ref left, ref right, op.pos);
                        left = GenerateSubtract(left, right);
                        break;
                    case TokenId.Amphersand:
                    amphersand:
                        left = GenerateStringConcat(left, right);
                        break;
                }
            }
            return left;
        }

        // *, /, %, mod operators
        public Expression ParseMultiplicative()
        {
            var left = ParseUnary();
            while (tokenVal.id == TokenId.Asterisk || tokenVal.id == TokenId.Slash || tokenVal.id == TokenId.Percent || TokenIdentifierIs("mod"))
            {
                var op = tokenVal;
                NextToken();
                var right = ParseUnary();
                CheckAndPromoteOperands(typeof(IArithmeticSignatures), op.text, ref left, ref right, op.pos);
                switch (op.id)
                {
                    case TokenId.Asterisk:
                        left = Expression.Multiply(left, right);
                        break;
                    case TokenId.Slash:
                        left = Expression.Divide(left, right);
                        break;
                    case TokenId.Percent:
                    case TokenId.Identifier:
                        left = Expression.Modulo(left, right);
                        break;
                }
            }
            return left;
        }

        // -, !, not unary operators
        public Expression ParseUnary()
        {
            if (tokenVal.id == TokenId.Minus || tokenVal.id == TokenId.Exclamation || TokenIdentifierIs("not"))
            {
                var op = tokenVal;
                NextToken();
                if (op.id == TokenId.Minus && (tokenVal.id == TokenId.IntegerLiteral || tokenVal.id == TokenId.RealLiteral))
                {
                    tokenVal.text = "-" + tokenVal.text;
                    tokenVal.pos = op.pos;
                    return ParsePrimary();
                }
                var expr = ParseUnary();
                if (op.id == TokenId.Minus)
                {
                    CheckAndPromoteOperand(typeof(INegationSignatures), op.text, ref expr, op.pos);
                    expr = Expression.Negate(expr);
                }
                else
                {
                    CheckAndPromoteOperand(typeof(INotSignatures), op.text, ref expr, op.pos);
                    expr = Expression.Not(expr);
                }
                return expr;
            }
            return ParsePrimary();
        }

        public Expression ParsePrimary()
        {
            var expr = ParsePrimaryStart();
            do
            {
                if (tokenVal.id == TokenId.Dot)
                {
                    NextToken();
                    expr = ParseMemberAccess(null, expr);
                }
                else if (tokenVal.id == TokenId.OpenBracket)
                {
                    expr = ParseElementAccess(expr);
                }
                else
                {
                    break; // TODO: might not be correct. Was : Exit Do
                }
            } while (true);
            return expr;
        }

        public Expression ParsePrimaryStart()
        {
            switch (tokenVal.id)
            {
                case TokenId.Identifier:
                    return ParseIdentifier();
                case TokenId.StringLiteral:
                    return ParseStringLiteral();
                case TokenId.IntegerLiteral:
                    return ParseIntegerLiteral();
                case TokenId.RealLiteral:
                    return ParseRealLiteral();
                case TokenId.OpenParen:
                    return ParseParenExpression();
                default:
                    throw ParseError(Res.ExpressionExpected);
            }
        }

        public Expression ParseStringLiteral()
        {
            ValidateToken(TokenId.StringLiteral);

            var quote = tokenVal.text[0];
            var s = tokenVal.text.Substring(1, tokenVal.text.Length - 2);
            var start = 0;

            do
            {
                var i = s.IndexOf(quote, start);
                if (i < 0)
                    break; // TODO: might not be correct. Was : Exit Do
                s = s.Remove(i, 1);
                start = i + 1;
            } while (true);

            if (quote == "'")
            {
                if (s.Length != 1)
                {
                    throw ParseError(Res.InvalidCharacterLiteral);
                }
                NextToken();
                return CreateLiteral(s[0], s);
            }
            NextToken();
            return CreateLiteral(s, s);
        }

        public Expression ParseIntegerLiteral()
        {
            ValidateToken(TokenId.IntegerLiteral);
            var text = tokenVal.text;
            if (text[0] != "-")
            {
                ulong value = 0;
                if (!UInt64.TryParse(text, out value))
                {
                    throw ParseError(Res.InvalidIntegerLiteral, text);
                }

                NextToken();
                if (value <= Convert.ToUInt64(Int32.MaxValue))
                    return CreateLiteral(Convert.ToInt32(value), text);
                if (value <= Convert.ToUInt64(UInt32.MaxValue))
                    return CreateLiteral(Convert.ToUInt32(value), text);
                if (value <= Convert.ToUInt64(Int64.MaxValue))
                    return CreateLiteral(Convert.ToInt64(value), text);
                return CreateLiteral(value, text);
            }
            else
            {
                long value = 0;
                if (!Int64.TryParse(text, out value))
                {
                    throw ParseError(Res.InvalidIntegerLiteral, text);
                }
                NextToken();
                if ((value >= Int32.MinValue && value <= Int32.MaxValue))
                {
                    return CreateLiteral(Convert.ToInt32(value), text);
                }
                return CreateLiteral(value, text);
            }
        }

        public Expression ParseRealLiteral()
        {
            ValidateToken(TokenId.RealLiteral);
            var text = tokenVal.text;
            object value = null;
            var last = text[text.Length - 1];
            if (last == "f" | last == "F")
            {
                float f = 0;
                if (float.TryParse(text.Substring(0, text.Length - 1), out f))
                    value = f;
            }
            else
            {
                double d = 0;
                if (double.TryParse(text, out d))
                    value = d;
            }

            if (value == null)
                throw ParseError(Res.InvalidRealLiteral, text);
            NextToken();
            return CreateLiteral(value, text);
        }

        public Expression CreateLiteral(object value, string text)
        {
            var expr = Expression.Constant(value);
            literals.Add(expr, text);
            return expr;
        }

        public Expression ParseParenExpression()
        {
            ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
            NextToken();
            var e = ParseExpression();
            ValidateToken(TokenId.CloseParen, Res.CloseParenOrOperatorExpected);
            NextToken();
            return e;
        }

        public Expression ParseIdentifier()
        {
            ValidateToken(TokenId.Identifier);
            object value = null;
            if (keywords.TryGetValue(tokenVal.text, out value))
            {
                if (value as Type != null)
                    return ParseTypeAccess((Type)value);
                if (object.ReferenceEquals(value, keywordIt))
                    return ParseIt();
                if (object.ReferenceEquals(value, keywordIif))
                    return ParseIif();
                if (object.ReferenceEquals(value, keywordNew))
                    return ParseNew();
                NextToken();
                return (Expression)value;
            }

            if (symbols.TryGetValue(tokenVal.text, out value) || externals != null && externals.TryGetValue(tokenVal.text, out value))
            {
                var expr = value as Expression;
                if (expr == null)
                {
                    expr = Expression.Constant(value);
                }
                else
                {
                    var lambda = expr as LambdaExpression;
                    if (lambda != null)
                        return ParseLambdaInvocation(lambda);
                }
                NextToken();
                return expr;
            }
            if (it != null)
                return ParseMemberAccess(null, it);
            throw ParseError(Res.UnknownIdentifier, tokenVal.text);
        }

        public Expression ParseIt()
        {
            if (it == null)
                throw ParseError(Res.NoItInScope);
            NextToken();
            return it;
        }

        public Expression ParseIif()
        {
            var errorPos = tokenVal.pos;
            NextToken();
            Expression[] args = ParseArgumentList();
            if (args.Length != 3)
            {
                throw ParseError(errorPos, Res.IifRequiresThreeArgs);
            }
            return GenerateConditional(args[0], args[1], args[2], errorPos);
        }

        public Expression GenerateConditional(Expression test, Expression expr1, Expression expr2, int errorPos)
        {
            if (!test.Type.Equals(typeof(bool)))
            {
                throw ParseError(errorPos, Res.FirstExprMustBeBool);
            }
            if (!expr1.Type.Equals(expr2.Type))
            {
                Expression expr1as2 = !expr2.Equals(nullLiteral) ? PromoteExpression(expr1, expr2.Type, true) : null;
                Expression expr2as1 = !expr1.Equals(nullLiteral) ? PromoteExpression(expr2, expr1.Type, true) : null;
                if (expr1as2 != null & expr2as1 == null)
                {
                    expr1 = expr1as2;
                }
                else if (expr2as1 != null & expr1as2 == null)
                {
                    expr2 = expr2as1;
                }
                else
                {
                    var type1 = !expr1.Equals(nullLiteral) ? expr1.Type.Name : "null";
                    var type2 = !expr2.Equals(nullLiteral) ? expr2.Type.Name : "null";
                    if (expr1as2 != null & expr2as1 != null)
                    {
                        throw ParseError(errorPos, Res.BothTypesConvertToOther, type1, type2);
                    }
                    throw ParseError(errorPos, Res.NeitherTypeConvertsToOther, type1, type2);
                }
            }
            return Expression.Condition(test, expr1, expr2);
        }

        public Expression ParseNew()
        {
            NextToken();
            ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
            NextToken();
            List<DynamicProperty> properties = new List<DynamicProperty>();
            List<Expression> expressions = new List<Expression>();
            do
            {
                var exprPos = tokenVal.pos;
                var expr = ParseExpression();
                string propName = null;
                if (TokenIdentifierIs("as"))
                {
                    NextToken();
                    propName = GetIdentifier();
                    NextToken();
                }
                else
                {
                    MemberExpression me = expr as MemberExpression;
                    if (me == null)
                        throw ParseError(exprPos, Res.MissingAsClause);
                    propName = me.Member.Name;
                }
                expressions.Add(expr);
                properties.Add(new DynamicProperty(propName, expr.Type));
                if (tokenVal.id != TokenId.Comma)
                    break; // TODO: might not be correct. Was : Exit Do
                NextToken();
            } while (true);
            ValidateToken(TokenId.CloseParen, Res.CloseParenOrCommaExpected);
            NextToken();
            Type type = DynamicExpression.CreateClass(properties);
            MemberBinding[] bindings = new MemberBinding[properties.Count];
            for (int i = 0; i <= bindings.Length - 1; i++)
            {
                bindings[i] = Expression.Bind(type.GetProperty(properties[i].Name), expressions[i]);
            }
            return Expression.MemberInit(Expression.New(type), bindings);
        }

        public Expression ParseLambdaInvocation(LambdaExpression lambda)
        {
            var errorPos = tokenVal.pos;
            NextToken();
            Expression[] args = ParseArgumentList();
            MethodBase method = null;
            if (FindMethod(lambda.Type, "Invoke", false, args, ref method) != 1)
            {
                throw ParseError(errorPos, Res.ArgsIncompatibleWithLambda);
            }
            return Expression.Invoke(lambda, args);
        }

        public Expression ParseTypeAccess(Type type)
        {
            var errorPos = tokenVal.pos;
            NextToken();

            if (tokenVal.id == TokenId.Question)
            {
                if ((!type.IsValueType) || IsNullableType(type))
                {
                    throw ParseError(errorPos, Res.TypeHasNoNullableForm, GetTypeName(type));
                }
                type = typeof(Nullable<int>).GetGenericTypeDefinition().MakeGenericType(type);
                NextToken();
            }
            if (tokenVal.id == TokenId.OpenParen)
            {
                Expression[] args = ParseArgumentList();
                MethodBase method = null;
                switch (FindBestMethod(type.GetConstructors(), args, ref method))
                {
                    case 0:
                        if (args.Length == 1)
                        {
                            return GenerateConversion(args[0], type, errorPos);
                        }
                        throw ParseError(errorPos, Res.NoMatchingConstructor, GetTypeName(type));
                    case 1:
                        return Expression.New((ConstructorInfo)method, args);
                    default:
                        throw ParseError(errorPos, Res.AmbiguousConstructorInvocation, GetTypeName(type));
                }
            }
            ValidateToken(TokenId.Dot, Res.DotOrOpenParenExpected);
            NextToken();
            return ParseMemberAccess(type, null);
        }

        public Expression GenerateConversion(Expression expr, Type type, int errorPos)
        {
            var exprType = expr.Type;
            if (exprType.Equals(type))
                return expr;
            if (exprType.IsValueType && type.IsValueType)
            {
                if ((IsNullableType(exprType) || IsNullableType(type)) && GetNonNullableType(exprType).Equals(GetNonNullableType(type)))
                {
                    return Expression.Convert(expr, type);
                }

                if ((IsNumericType(exprType) || IsEnumType(exprType)) && (IsNumericType(type) || IsEnumType(type)))
                {
                    return Expression.ConvertChecked(expr, type);
                }
            }
            if (exprType.IsAssignableFrom(type) || type.IsAssignableFrom(exprType) || exprType.IsInterface || type.IsInterface)
            {
                return Expression.Convert(expr, type);
            }
            throw ParseError(errorPos, Res.CannotConvertValue, GetTypeName(exprType), GetTypeName(type));
        }

        public Expression ParseMemberAccess(Type type, Expression instance)
        {
            if (instance != null)
                type = instance.Type;
            var errorPos = tokenVal.pos;
            var id = GetIdentifier();
            NextToken();
            if (tokenVal.id == TokenId.OpenParen)
            {
                if (instance != null && !type.Equals(typeof(string)))
                {
                    Type enumerableType = FindGenericType(typeof(IEnumerable<object>).GetGenericTypeDefinition(), type);
                    if (enumerableType != null)
                    {
                        Type elementType = enumerableType.GetGenericArguments()[0];
                        return ParseAggregate(instance, elementType, id, errorPos);
                    }
                }
                Expression[] args = ParseArgumentList();
                MethodBase mb = null;
                switch (FindMethod(type, id, instance == null, args, ref mb))
                {
                    case 0:
                        throw ParseError(errorPos, Res.NoApplicableMethod, id, GetTypeName(type));
                    case 1:
                        var method = (MethodInfo)mb;
                        if ((!IsPredefinedType(method.DeclaringType)))
                        {
                            throw ParseError(errorPos, Res.MethodsAreInaccessible, GetTypeName(method.DeclaringType));
                        }
                        if (method.ReturnType.Equals(typeof(Void)))
                        {
                            throw ParseError(errorPos, Res.MethodIsVoid, id, GetTypeName(method.DeclaringType));
                        }
                        return Expression.Call(instance, (MethodInfo)method, args);
                    default:
                        throw ParseError(errorPos, Res.AmbiguousMethodInvocation, id, GetTypeName(type));
                }
            }
            else
            {
                MemberInfo member = FindPropertyOrField(type, id, instance == null);
                if (member == null)
                {
                    throw ParseError(errorPos, Res.UnknownPropertyOrField, id, GetTypeName(type));
                }
                return member as PropertyInfo != null ? Expression.Property(instance, (PropertyInfo)member) : Expression.Field(instance, (FieldInfo)member);
            }
        }

        public static Type FindGenericType(Type generic, Type type)
        {
            while (type != null && !type.Equals(typeof(object)))
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(generic))
                    return type;
                if (generic.IsInterface)
                {
                    foreach (Type intfType in type.GetInterfaces())
                    {
                        Type found = FindGenericType(generic, intfType);
                        if (found != null)
                            return found;
                    }
                }
                type = type.BaseType;
            }
            return null;
        }

        public Expression ParseAggregate(Expression instance, Type elementType, string methodName, int errorPos)
        {
            ParameterExpression outerIt = it;
            ParameterExpression innerIt = Expression.Parameter(elementType, "");
            it = innerIt;
            Expression[] args = ParseArgumentList();
            it = outerIt;
            MethodBase signature = null;
            if (FindMethod(typeof(IEnumerableSignatures), methodName, false, args, ref signature) != 1)
            {
                throw ParseError(errorPos, Res.NoApplicableAggregate, methodName);
            }
            Type[] typeArgs = null;
            if (signature.Name == "Min" || signature.Name == "Max")
            {
                typeArgs = new Type[] {
					elementType,
					args[0].Type
				};
            }
            else
            {
                typeArgs = new Type[] { elementType };
            }

            if (args.Length == 0)
            {
                args = new Expression[] { instance };
            }
            else
            {
                args = new Expression[] {
					instance,
					Expression.Lambda(args[0], innerIt)
				};
            }
            return Expression.Call(typeof(Enumerable), signature.Name, typeArgs, args);
        }

        public Expression[] ParseArgumentList()
        {
            ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
            NextToken();
            Expression[] args = tokenVal.id != TokenId.CloseParen ? ParseArguments() : new Expression[-1 + 1];
            ValidateToken(TokenId.CloseParen, Res.CloseParenOrCommaExpected);
            NextToken();
            return args;
        }

        public Expression[] ParseArguments()
        {
            List<Expression> argList = new List<Expression>();
            do
            {
                argList.Add(ParseExpression());
                if (tokenVal.id != TokenId.Comma)
                    break; // TODO: might not be correct. Was : Exit Do
                NextToken();
            } while (true);
            return argList.ToArray();
        }

        public Expression ParseElementAccess(Expression expr)
        {
            int errorPos = tokenVal.pos;
            ValidateToken(TokenId.OpenBracket, Res.OpenParenExpected);
            NextToken();
            Expression[] args = ParseArguments();
            ValidateToken(TokenId.CloseBracket, Res.CloseBracketOrCommaExpected);
            NextToken();
            if (expr.Type.IsArray)
            {
                if (expr.Type.GetArrayRank() != 1 || args.Length != 1)
                {
                    throw ParseError(errorPos, Res.CannotIndexMultiDimArray);
                }
                Expression index = PromoteExpression(args[0], typeof(int), true);
                if (index == null)
                {
                    throw ParseError(errorPos, Res.InvalidIndex);
                }
                return Expression.ArrayIndex(expr, index);
            }
            else
            {
                MethodBase mb = null;
                switch (FindIndexer(expr.Type, args, ref mb))
                {
                    case 0:
                        throw ParseError(errorPos, Res.NoApplicableIndexer, GetTypeName(expr.Type));
                    case 1:
                        return Expression.Call(expr, (MethodInfo)mb, args);
                    default:
                        throw ParseError(errorPos, Res.AmbiguousIndexerInvocation, GetTypeName(expr.Type));
                }
            }
        }

        public static bool IsPredefinedType(Type type)
        {
            foreach (Type t in predefinedTypes)
            {
                if (t.Equals(type))
                    return true;
            }

            return false;
        }

        public static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<int>).GetGenericTypeDefinition());
        }

        public static Type GetNonNullableType(Type type)
        {
            return IsNullableType(type) ? type.GetGenericArguments()[0] : type;
        }

        public static string GetTypeName(Type type)
        {
            var baseType = GetNonNullableType(type);
            var s = baseType.Name;
            if (!type.Equals(baseType))
                s += "?";
            return s;
        }

        public static bool IsNumericType(Type type)
        {
            return GetNumericTypeKind(type) != 0;
        }

        public static bool IsSignedIntegralType(Type type)
        {
            return GetNumericTypeKind(type) == 2;
        }

        public static bool IsUnsignedIntegralType(Type type)
        {
            return GetNumericTypeKind(type) == 3;
        }

        public static int GetNumericTypeKind(Type type)
        {
            type = GetNonNullableType(type);
            if (type.IsEnum)
                return 0;
            switch (type.GetTypeCode(type))
            {
                case TypeCode.Char:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return 1;
                case TypeCode.SByte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                    return 2;
                case TypeCode.Byte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return 3;
                default:
                    return 0;
            }
        }

        public static bool IsEnumType(Type type)
        {
            return GetNonNullableType(type).IsEnum;
        }

        public void CheckAndPromoteOperand(Type signatures, string opName, ref Expression expr, int errorPos)
        {
            Expression[] args = new Expression[] { expr };
            MethodBase method = null;
            if (FindMethod(signatures, "F", false, args, ref method) != 1)
            {
                throw ParseError(errorPos, Res.IncompatibleOperand, opName, GetTypeName(args[0].Type));
            }
            expr = args[0];
        }

        public void CheckAndPromoteOperands(Type signatures, string opName, ref Expression left, ref Expression right, int errorPos)
        {
            Expression[] args = new Expression[] {
				left,
				right
			};
            MethodBase method = null;
            if (FindMethod(signatures, "F", false, args, ref method) != 1)
            {
                throw IncompatibleOperandsError(opName, left, right, errorPos);
            }
            left = args[0];
            right = args[1];
        }

        public Exception IncompatibleOperandsError(string opName, Expression left, Expression right, int pos)
        {
            return ParseError(pos, Res.IncompatibleOperands, opName, GetTypeName(left.Type), GetTypeName(right.Type));
        }

        public MemberInfo FindPropertyOrField(Type type, string memberName, bool staticAccess)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.DeclaredOnly | staticAccess ? BindingFlags.Static : BindingFlags.Instance;
            foreach (Type t in SelfAndBaseTypes(type))
            {
                MemberInfo[] members = t.FindMembers(MemberTypes.Property | MemberTypes.Field, flags, type.FilterNameIgnoreCase, memberName);
                if (members.Length != 0)
                    return members[0];
            }
            return null;
        }

        public int FindMethod(Type type, string methodName, bool staticAccess, Expression[] args, ref MethodBase method)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.DeclaredOnly | staticAccess ? BindingFlags.Static : BindingFlags.Instance;
            foreach (Type t in SelfAndBaseTypes(type))
            {
                MemberInfo[] members = t.FindMembers(MemberTypes.Method, flags, type.FilterNameIgnoreCase, methodName);
                int count = FindBestMethod(members.Cast<MethodBase>(), args, ref method);
                if (count != 0)
                    return count;
            }
            method = null;
            return 0;
        }

        public int FindIndexer(Type type, Expression[] args, ref MethodBase method)
        {
            foreach (Type t in SelfAndBaseTypes(type))
            {
                MemberInfo[] members = t.GetDefaultMembers();
                if (members.Length != 0)
                {
                    IEnumerable<MethodBase> methods = members.OfType<PropertyInfo>().Select(p => (MethodBase)p.GetGetMethod()).Where(m => m != null);
                    int count = FindBestMethod(methods, args, ref method);
                    if (count != 0)
                        return count;
                }
            }
            method = null;
            return 0;
        }

        public static IEnumerable<Type> SelfAndBaseTypes(Type type)
        {
            if (type.IsInterface)
            {
                List<Type> types = new List<Type>();
                AddInterface(types, type);
                return types;
            }
            return SelfAndBaseClasses(type);
        }

        public static IEnumerable<Type> SelfAndBaseClasses(Type type)
        {
            LinkedList<Type> results = new LinkedList<Type>();

            while (type != null)
            {
                results.AddLast(type);
                type = type.BaseType;
            }

            return results;
        }

        public static void AddInterface(List<Type> types, Type type)
        {
            if (!types.Contains(type))
            {
                types.Add(type);
            }
            foreach (Type t in type.GetInterfaces())
            {
                AddInterface(types, t);
            }
        }

        public class MethodData
        {
            public MethodBase MethodBase;
            public ParameterInfo[] Parameters;
            public Expression[] Args;
        }

        public int FindBestMethod(IEnumerable<MethodBase> methods, Expression[] args, ref MethodBase method)
        {
            MethodData[] applicable = methods.Select(m => new MethodData
            {
                MethodBase = m,
                Parameters = m.GetParameters()
            }).Where(m => IsApplicable(m, args)).ToArray();
            if (applicable.Length > 1)
            {
                applicable = applicable.Where(m => applicable.All(n => object.ReferenceEquals(m, n) || IsBetterThan(args, m, n))).ToArray();
            }
            if (applicable.Length == 1)
            {
                MethodData md = applicable[0];
                for (int i = 0; i <= args.Length - 1; i++)
                {
                    args[i] = md.Args[i];
                }
                method = md.MethodBase;
            }
            else
            {
                method = null;
            }
            return applicable.Length;
        }

        public bool IsApplicable(MethodData method, Expression[] args)
        {
            if (method.Parameters.Length != args.Length)
                return false;
            Expression[] promotedArgs = new Expression[args.Length];

            for (int i = 0; i <= args.Length - 1; i++)
            {
                ParameterInfo pi = method.Parameters[i];
                if (pi.IsOut)
                    return false;
                Expression promoted = PromoteExpression(args[i], pi.ParameterType, false);
                if (promoted == null)
                    return false;
                promotedArgs[i] = promoted;
            }
            method.Args = promotedArgs;

            return true;
        }

        public Expression PromoteExpression(Expression expr, Type type, bool exact)
        {
            if (expr.Type.Equals(type))
                return expr;
            if (expr as ConstantExpression != null)
            {
                var ce = (ConstantExpression)expr;
                if (ce.Equals(nullLiteral))
                {
                    if (!type.IsValueType || IsNullableType(type))
                    {
                        return Expression.Constant(null, type);
                    }
                }
                else
                {
                    string text = null;
                    if (literals.TryGetValue(ce, out text))
                    {
                        Type target = GetNonNullableType(type);
                        object value = null;
                        switch (type.GetTypeCode(ce.Type))
                        {
                            case TypeCode.Int32:
                            case TypeCode.UInt32:
                            case TypeCode.Int64:
                            case TypeCode.UInt64:
                                value = ParseNumber(text, target);
                                break;
                            case TypeCode.Double:
                                if (target.Equals(typeof(decimal)))
                                    value = ParseNumber(text, target);
                                break;
                            case TypeCode.String:
                                value = ParseEnum(text, target);
                                break;
                        }
                        try
                        {
                            if (value != null)
                                return Expression.Constant(value, type);
                        }
                        catch (Exception ex)
                        {
                            throw ParseError(Res.ExpressionExpected);
                        }
                    }
                }
            }

            if (IsCompatibleWith(expr.Type, type))
            {
                if (type.IsValueType || exact)
                    return Expression.Convert(expr, type);
                return expr;
            }
            return null;
        }

        public static object ParseNumber(string text, Type type)
        {
            switch (type.GetTypeCode(GetNonNullableType(type)))
            {
                case TypeCode.SByte:
                    sbyte sb = 0;
                    if (sbyte.TryParse(text, out sb))
                        return sb;
                    break;
                case TypeCode.Byte:
                    byte b = 0;
                    if (byte.TryParse(text, out b))
                        return b;
                    break;
                case TypeCode.Int16:
                    short s = 0;
                    if (short.TryParse(text, out s))
                        return s;
                    break;
                case TypeCode.UInt16:
                    ushort us = 0;
                    if (ushort.TryParse(text, out us))
                        return us;
                    break;
                case TypeCode.Int32:
                    int i = 0;
                    if (int.TryParse(text, out i))
                        return i;
                    break;
                case TypeCode.UInt32:
                    uint ui = 0;
                    if (uint.TryParse(text, out ui))
                        return ui;
                    break;
                case TypeCode.Int64:
                    long l = 0;
                    if (long.TryParse(text, out l))
                        return l;
                    break;
                case TypeCode.UInt64:
                    ulong ul = 0;
                    if (ulong.TryParse(text, out ul))
                        return ul;
                    break;
                case TypeCode.Single:
                    float f = 0;
                    if (float.TryParse(text, out f))
                        return f;
                    break;
                case TypeCode.Double:
                    double d = 0;
                    if (double.TryParse(text, out d))
                        return d;
                    break;
                case TypeCode.Decimal:
                    decimal e = default(decimal);
                    if (decimal.TryParse(text, out e))
                        return e;
                    break;
            }
            return null;
        }

        public static object ParseEnum(string name, Type type)
        {
            if (type.IsEnum)
            {
                MemberInfo[] memberInfos = type.FindMembers(MemberTypes.Field, BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static, type.FilterNameIgnoreCase, name);
                if (memberInfos.Length != 0)
                    return ((FieldInfo)memberInfos[0]).GetValue(null);
            }
            return null;
        }

        public static bool IsCompatibleWith(Type source, Type target)
        {
            if (source.Equals(target))
                return true;
            if (!target.IsValueType)
                return target.IsAssignableFrom(source);
            Type st = GetNonNullableType(source);
            Type tt = GetNonNullableType(target);
            if (!st.Equals(source) && tt.Equals(target))
                return false;
            TypeCode sc = st.IsEnum ? TypeCode.Object : System.Type.GetTypeCode(st);
            TypeCode tc = tt.IsEnum ? TypeCode.Object : System.Type.GetTypeCode(tt);

            switch (sc)
            {
                case TypeCode.SByte:
                    switch (tc)
                    {
                        case TypeCode.SByte:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Byte:
                    switch (tc)
                    {
                        case TypeCode.Byte:
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int16:
                    switch (tc)
                    {
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt16:
                    switch (tc)
                    {
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int32:
                    switch (tc)
                    {
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt32:
                    switch (tc)
                    {
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int64:
                    switch (tc)
                    {
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt64:
                    switch (tc)
                    {
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Single:
                    switch (tc)
                    {
                        case TypeCode.Single:
                        case TypeCode.Double:
                            return true;
                    }
                    break;
                default:
                    if (st.Equals(tt))
                        return true;
                    break;
            }
            return false;
        }

        public static bool IsBetterThan(Expression[] args, MethodData m1, MethodData m2)
        {
            var better = false;
            for (int i = 0; i <= args.Length - 1; i++)
            {
                int c = CompareConversions(args[i].Type, m1.Parameters[i].ParameterType, m2.Parameters[i].ParameterType);
                if (c < 0)
                    return false;
                if (c > 0)
                    better = true;
            }
            return better;
        }

        // Return 1 if s -> t1 is a better conversion than s -> t2
        // Return -1 if s -> t2 is a better conversion than s -> t1
        // Return 0 if neither conversion is better
        public static int CompareConversions(Type s, Type t1, Type t2)
        {
            if (t1.Equals(t2))
                return 0;
            if (s.Equals(t1))
                return 1;
            if (s.Equals(t2))
                return -1;
            bool t1t2 = IsCompatibleWith(t1, t2);
            bool t2t1 = IsCompatibleWith(t2, t1);
            if (t1t2 && !t2t1)
                return 1;
            if (t2t1 && !t1t2)
                return -1;
            if (IsSignedIntegralType(t1) && IsUnsignedIntegralType(t2))
                return 1;
            if (IsSignedIntegralType(t2) && IsUnsignedIntegralType(t1))
                return -1;
            return 0;
        }

        public Expression GenerateEqual(Expression left, Expression right)
        {
            return Expression.Equal(left, right);
        }

        public Expression GenerateNotEqual(Expression left, Expression right)
        {
            return Expression.NotEqual(left, right);
        }

        public Expression GenerateGreaterThan(Expression left, Expression right)
        {
            if (left.Type.Equals(typeof(string)))
            {
                return Expression.GreaterThan(GenerateStaticMethodCall("Compare", left, right), Expression.Constant(0));
            }
            return Expression.GreaterThan(left, right);
        }

        public Expression GenerateGreaterThanEqual(Expression left, Expression right)
        {
            if (left.Type.Equals(typeof(string)))
            {
                return Expression.GreaterThanOrEqual(GenerateStaticMethodCall("Compare", left, right), Expression.Constant(0));
            }
            return Expression.GreaterThanOrEqual(left, right);
        }

        public Expression GenerateLessThan(Expression left, Expression right)
        {
            if (left.Type.Equals(typeof(string)))
            {
                return Expression.LessThan(GenerateStaticMethodCall("Compare", left, right), Expression.Constant(0));
            }
            return Expression.LessThan(left, right);
        }

        public Expression GenerateLessThanEqual(Expression left, Expression right)
        {
            if (left.Type.Equals(typeof(string)))
            {
                return Expression.LessThanOrEqual(GenerateStaticMethodCall("Compare", left, right), Expression.Constant(0));
            }
            return Expression.LessThanOrEqual(left, right);
        }

        public Expression GenerateAdd(Expression left, Expression right)
        {
            if (left.Type.Equals(typeof(string)) && right.Type.Equals(typeof(string)))
            {
                return GenerateStaticMethodCall("Concat", left, right);
            }
            return Expression.Add(left, right);
        }

        public Expression GenerateSubtract(Expression left, Expression right)
        {
            return Expression.Subtract(left, right);
        }

        public Expression GenerateStringConcat(Expression left, Expression right)
        {
            return Expression.Call(null, typeof(string).GetMethod("Concat", new Type[] {
				typeof(object),
				typeof(object)
			}), new Expression[] {
				left,
				right
			});
        }

        public MethodInfo GetStaticMethod(string methodName, Expression left, Expression right)
        {
            return left.Type.GetMethod(methodName, new Type[] {
				left.Type,
				right.Type
			});
        }

        public Expression GenerateStaticMethodCall(string methodName, Expression left, Expression right)
        {
            return Expression.Call(null, GetStaticMethod(methodName, left, right), new Expression[] {
				left,
				right
			});
        }

        public void SetTextPos(int pos)
        {
            textPos = pos;
            ch = textPos < textLen ? text[textPos] : Strings.ChrW(0);
        }

        public void NextChar()
        {
            if (textPos < textLen)
                textPos += 1;
            ch = textPos < textLen ? text[textPos] : Strings.ChrW(0);
        }

        public void NextToken()
        {
            while (char.IsWhiteSpace(ch))
            {
                NextChar();
            }

            TokenId t = default(TokenId);
            var tokenPos = textPos;
            switch (ch)
            {
                case '!':
                    NextChar();
                    if (ch == "=")
                    {
                        NextChar();
                        t = TokenId.ExclamationEqual;
                    }
                    else
                    {
                        t = TokenId.Exclamation;
                    }
                    break;
                case '%':
                    NextChar();
                    t = TokenId.Percent;
                    break;
                case '&':
                    NextChar();
                    if (ch == "&")
                    {
                        NextChar();
                        t = TokenId.DoubleAmphersand;
                    }
                    else
                    {
                        t = TokenId.Amphersand;
                    }
                    break;
                case '(':
                    NextChar();
                    t = TokenId.OpenParen;
                    break;
                case ')':
                    NextChar();
                    t = TokenId.CloseParen;
                    break;
                case '*':
                    NextChar();
                    t = TokenId.Asterisk;
                    break;
                case '+':
                    NextChar();
                    t = TokenId.Plus;
                    break;
                case ',':
                    NextChar();
                    t = TokenId.Comma;
                    break;
                case '-':
                    NextChar();
                    t = TokenId.Minus;
                    break;
                case '.':
                    NextChar();
                    t = TokenId.Dot;
                    break;
                case '/':
                    NextChar();
                    t = TokenId.Slash;
                    break;
                case ':':
                    NextChar();
                    t = TokenId.Colon;
                    break;
                case '<':
                    NextChar();
                    if (ch == "=")
                    {
                        NextChar();
                        t = TokenId.LessThanEqual;
                    }
                    else if (ch == ">")
                    {
                        NextChar();
                        t = TokenId.LessGreater;
                    }
                    else
                    {
                        t = TokenId.LessThan;
                    }
                    break;
                case '=':
                    NextChar();
                    if (ch == "=")
                    {
                        NextChar();
                        t = TokenId.DoubleEqual;
                    }
                    else
                    {
                        t = TokenId.Equal;
                    }
                    break;
                case '>':
                    NextChar();
                    if (ch == "=")
                    {
                        NextChar();
                        t = TokenId.GreaterThanEqual;
                    }
                    else
                    {
                        t = TokenId.GreaterThan;
                    }
                    break;
                case '?':
                    NextChar();
                    t = TokenId.Question;
                    break;
                case '[':
                    NextChar();
                    t = TokenId.OpenBracket;
                    break;
                case ']':
                    NextChar();
                    t = TokenId.CloseBracket;
                    break;
                case '|':
                    NextChar();
                    if (ch == "|")
                    {
                        NextChar();
                        t = TokenId.DoubleBar;
                    }
                    else
                    {
                        t = TokenId.Bar;
                    }
                    break;
                case '\'':
                case '"':
                    var quote = ch;
                    do
                    {
                        NextChar();
                        while (textPos < textLen && ch != quote)
                        {
                            NextChar();
                        }
                        if (textPos == textLen)
                            throw ParseError(textPos, Res.UnterminatedStringLiteral);
                        NextChar();
                    } while (ch == quote);

                    t = TokenId.StringLiteral;
                    break;
                default:
                    if (char.IsLetter(ch) || ch == "@" || ch == "_")
                    {
                        do
                        {
                            NextChar();
                        } while (char.IsLetterOrDigit(ch) || ch == "_");
                        t = TokenId.Identifier;
                        break; // TODO: might not be correct. Was : Exit Select
                    }

                    if (char.IsDigit(ch))
                    {
                        t = TokenId.IntegerLiteral;
                        do
                        {
                            NextChar();
                        } while (char.IsDigit(ch));
                        if (ch == ".")
                        {
                            t = TokenId.RealLiteral;
                            NextChar();
                            ValidateDigit();
                            do
                            {
                                NextChar();
                            } while (char.IsDigit(ch));
                        }
                        if (ch == "E" || ch == "e")
                        {
                            t = TokenId.RealLiteral;
                            NextChar();
                            if (ch == "+" || ch == "-")
                                NextChar();
                            ValidateDigit();
                            do
                            {
                                NextChar();
                            } while (char.IsDigit(ch));
                        }
                        if (ch == "F" | ch == "f")
                            NextChar();
                        break; // TODO: might not be correct. Was : Exit Select
                    }
                    if (textPos == textLen)
                    {
                        t = TokenId.End;
                        break; // TODO: might not be correct. Was : Exit Select
                    }
                    throw ParseError(textPos, Res.InvalidCharacter, ch);
            }
            tokenVal.id = t;
            tokenVal.text = text.Substring(tokenPos, textPos - tokenPos);
            tokenVal.pos = tokenPos;
        }

        public bool TokenIdentifierIs(string id)
        {
            return tokenVal.id == TokenId.Identifier && string.Equals(id, tokenVal.text, StringComparison.OrdinalIgnoreCase);
        }

        public string GetIdentifier()
        {
            ValidateToken(TokenId.Identifier, Res.IdentifierExpected);
            var id = tokenVal.text;
            if (id.Length > 1 && id[0] == "@")
                id = id.Substring(1);
            return id;
        }

        public void ValidateDigit()
        {
            if (!char.IsDigit(ch))
                throw ParseError(textPos, Res.DigitExpected);
        }

        public void ValidateToken(TokenId t, string errorMessage)
        {
            if (tokenVal.id != t)
                throw ParseError(errorMessage);
        }

        public void ValidateToken(TokenId t)
        {
            if (tokenVal.id != t)
                throw ParseError(Res.SyntaxError);
        }

        public Exception ParseError(string format, params object[] args)
        {
            return ParseError(tokenVal.pos, format, args);
        }

        public Exception ParseError(int pos, string format, params object[] args)
        {
            return new ParseException(string.Format(System.Globalization.CultureInfo.CurrentCulture, format, args), pos);
        }

        public static Dictionary<string, object> CreateKeywords()
        {
            Dictionary<string, object> d = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            d.Add("true", trueLiteral);
            d.Add("false", falseLiteral);
            d.Add("null", nullLiteral);
            d.Add(keywordIt, keywordIt);
            d.Add(keywordIif, keywordIif);
            d.Add(keywordNew, keywordNew);

            foreach (Type type in predefinedTypes)
            {
                d.Add(type.Name, type);
            }

            return d;
        }
    }
}

namespace Tween
{
    internal class Res
    {
        public const string DuplicateIdentifier = "The identifier '{0}' was defined more than once";
        public const string ExpressionTypeMismatch = "Expression of type '{0}' expected";
        public const string ExpressionExpected = "Expression expected";
        public const string InvalidCharacterLiteral = "Character literal must contain exactly one character";
        public const string InvalidIntegerLiteral = "Invalid integer literal '{0}'";
        public const string InvalidRealLiteral = "Invalid real literal '{0}'";
        public const string UnknownIdentifier = "Unknown identifier '{0}'";
        public const string NoItInScope = "No 'it' is in scope";
        public const string IifRequiresThreeArgs = "The 'iif' function requires three arguments";
        public const string FirstExprMustBeBool = "The first expression must be of type 'Boolean'";
        public const string BothTypesConvertToOther = "Both of the types '{0}' and '{1}' convert to the other";
        public const string NeitherTypeConvertsToOther = "Neither of the types '{0}' and '{1}' converts to the other";
        public const string MissingAsClause = "Expression is missing an 'as' clause";
        public const string ArgsIncompatibleWithLambda = "Argument list incompatible with lambda expression";
        public const string TypeHasNoNullableForm = "Type '{0}' has no nullable form";
        public const string NoMatchingConstructor = "No matching constructor in type '{0}'";
        public const string AmbiguousConstructorInvocation = "Ambiguous invocation of '{0}' constructor";
        public const string CannotConvertValue = "A value of type '{0}' cannot be converted to type '{1}'";
        public const string NoApplicableMethod = "No applicable method '{0}' exists in type '{1}'";
        public const string MethodsAreInaccessible = "Methods on type '{0}' are not accessible";
        public const string MethodIsVoid = "Method '{0}' in type '{1}' does not return a value";
        public const string AmbiguousMethodInvocation = "Ambiguous invocation of method '{0}' in type '{1}'";
        public const string UnknownPropertyOrField = "No property or field '{0}' exists in type '{1}'";
        public const string NoApplicableAggregate = "No applicable aggregate method '{0}' exists";
        public const string CannotIndexMultiDimArray = "Indexing of multi-dimensional arrays is not supported";
        public const string InvalidIndex = "Array index must be an integer expression";
        public const string NoApplicableIndexer = "No applicable indexer exists in type '{0}'";
        public const string AmbiguousIndexerInvocation = "Ambiguous invocation of indexer in type '{0}'";
        public const string IncompatibleOperand = "Operator '{0}' incompatible with operand type '{1}'";
        public const string IncompatibleOperands = "Operator '{0}' incompatible with operand types '{1}' and '{2}'";
        public const string UnterminatedStringLiteral = "Unterminated string literal";
        public const string InvalidCharacter = "Syntax error '{0}'";
        public const string DigitExpected = "Digit expected";
        public const string SyntaxError = "Syntax error";
        public const string TokenExpected = "{0} expected";
        public const string ParseExceptionFormat = "{0} (at index {1})";
        public const string ColonExpected = "':' expected";
        public const string OpenParenExpected = "'(' expected";
        public const string CloseParenOrOperatorExpected = "')' or operator expected";
        public const string CloseParenOrCommaExpected = "')' or ',' expected";
        public const string DotOrOpenParenExpected = "'.' or '(' expected";
        public const string OpenBracketExpected = "'[' expected";
        public const string CloseBracketOrCommaExpected = "']' or ',' expected";
        public const string IdentifierExpected = "Identifier expected";
    }
}