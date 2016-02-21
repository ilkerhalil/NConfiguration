using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace NConfiguration.Serialization
{
	public class ComplexFunctionBuilder
	{
		private Type _targetType;
		private bool _supportInitialize;
		private ParameterExpression _pDeserializer = Expression.Parameter(typeof(IDeserializer));
		private ParameterExpression _pCfgNode = Expression.Parameter(typeof(ICfgNode));
		private List<Expression> _bodyList = new List<Expression>();
		private ParameterExpression _pResult;

		public ComplexFunctionBuilder(Type targetType)
		{
			_targetType = targetType;
			_supportInitialize = typeof(ISupportInitialize).IsAssignableFrom(_targetType);
			_pResult = Expression.Parameter(_targetType);

			SetConstructor();
			if (_supportInitialize)
				CallBeginInit();
		}

		private void SetConstructor()
		{
			if (_targetType.IsValueType)
				return;
			
			var ci = _targetType.GetConstructor(
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
				null, new Type[] { }, null);

			Expression resultInstance;

			if (ci == null)
			{
				resultInstance = Expression.Call(typeof(FormatterServices).GetMethod("GetUninitializedObject"), Expression.Constant(_targetType));
				resultInstance = Expression.Convert(resultInstance, _targetType);
			}
			else
				resultInstance = Expression.New(ci);

			_bodyList.Add(Expression.Assign(_pResult, resultInstance));
		}

		public object Compile()
		{
			if (_supportInitialize)
				CallEndInit();

			_bodyList.Add(Expression.Label(Expression.Label(_targetType), _pResult));

			var delegateType = typeof(Deserialize<>).MakeGenericType(_targetType);

			return Expression.Lambda(delegateType, Expression.Block(new[] { _pResult }, _bodyList), _pDeserializer, _pCfgNode).Compile();
		}

		private void CallBeginInit()
		{
			var callBeginInit = Expression.Call(_pResult, typeof(ISupportInitialize).GetMethod("BeginInit"));
			_bodyList.Add(callBeginInit);
		}

		private void CallEndInit()
		{
			var callEndInit = Expression.Call(_pResult, typeof(ISupportInitialize).GetMethod("EndInit"));
			_bodyList.Add(callEndInit);
		}

		private Expression CreateFunction(FieldFunctionInfo ffi)
		{
			var dmAttr = ffi.CustomAttributes.OfType<DataMemberAttribute>().FirstOrDefault();
			
			if (dmAttr != null && !string.IsNullOrWhiteSpace(dmAttr.Name))
				ffi.Name = dmAttr.Name;
			
			if (dmAttr != null)
				ffi.Required = dmAttr.IsRequired;

			return MakeFieldReader(ffi);
		}

		private Expression MakeFieldReader(FieldFunctionInfo ffi)
		{
			if (!SimpleTypes.Converter.IsPrimitive(ffi.ResultType))
			{
				if (ffi.ResultType.IsArray)
				{
					var itemType = ffi.ResultType.GetElementType();
					var mi = BuildUtils.ToArrayMI.MakeGenericMethod(itemType);
					return Expression.Call(null, mi, _pDeserializer, Expression.Constant(ffi.Name), _pCfgNode);
				}

				if (BuildUtils.IsCollection(ffi.ResultType))
				{
					var itemType = ffi.ResultType.GetGenericArguments()[0];
					var mi = BuildUtils.ToListMI.MakeGenericMethod(itemType);
					return Expression.Call(null, mi, _pDeserializer, Expression.Constant(ffi.Name), _pCfgNode);
				}
			}

			{
				var mi = ffi.Required ? BuildUtils.RequiredFieldMI : BuildUtils.OptionalFieldMI;
				mi = mi.MakeGenericMethod(ffi.ResultType);
				return Expression.Call(null, mi, _pDeserializer, Expression.Constant(ffi.Name), _pCfgNode);
			}
		}

		internal void Add(FieldInfo fi)
		{
			if (fi.IsInitOnly)
				return;

			if (fi.IsPrivate)
			{ // require DataMemberAttribute
				if (fi.GetCustomAttribute<DataMemberAttribute>() == null)
					return;
			}

			if (fi.GetCustomAttribute<IgnoreDataMemberAttribute>() != null)
				return;

			var right = CreateFunction(new FieldFunctionInfo(fi));
			if (right == null)
				return;
			var left = Expression.Field(_pResult, fi);
			_bodyList.Add(Expression.Assign(left, right));
		}

		internal void Add(PropertyInfo pi)
		{
			if (!pi.CanWrite || !pi.CanRead)
				return;

			if (pi.SetMethod.IsPrivate && pi.GetMethod.IsPrivate)
			{ // require DataMemberAttribute
				if (pi.GetCustomAttribute<DataMemberAttribute>() == null)
					return;
			}

			if (pi.GetCustomAttribute<IgnoreDataMemberAttribute>() != null)
				return;

			var right = CreateFunction(new FieldFunctionInfo(pi));
			if (right == null)
				return;
			var left = Expression.Property(_pResult, pi);
			_bodyList.Add(Expression.Assign(left, right));
		}
	}
}
