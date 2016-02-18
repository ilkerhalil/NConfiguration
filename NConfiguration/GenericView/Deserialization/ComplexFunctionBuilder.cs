using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel;

namespace NConfiguration.Serialization.Deserialization
{
	public class ComplexFunctionBuilder
	{
		private Type _targetType;
		private bool _supportInitialize;
		private IGenericDeserializer _deserializer;
		private StringConverter _converter;
		private ParameterExpression _pCfgNode = Expression.Parameter(typeof(ICfgNode));
		private List<Expression> _bodyList = new List<Expression>();
		private ParameterExpression _pResult;
		private Action<FieldFunctionInfo> _configureFieldInfo;

		public ComplexFunctionBuilder(Type targetType, IGenericDeserializer deserializer, StringConverter converter, Action<FieldFunctionInfo> configureFieldInfo)
		{
			_targetType = targetType;
			_supportInitialize = typeof(ISupportInitialize).IsAssignableFrom(_targetType);
			_pResult = Expression.Parameter(_targetType);
			_deserializer = deserializer;
			_converter = converter;
			_configureFieldInfo = configureFieldInfo;
		}

		private void SetConstructor()
		{
			if (_targetType.IsValueType)
				return;
			
			var ci = _targetType.GetConstructor(
				BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
				null, new Type[] { }, null);
				
			if (ci == null)
				throw new InvalidOperationException("default constructor not found");

			_bodyList.Add(Expression.Assign(_pResult, Expression.New(ci)));
		}

		public object Compile()
		{
			try
			{
				SetConstructor();
				if (_supportInitialize)
					CallBeginInit();

				foreach (var fi in _targetType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					var right = CreateFunction(new FieldFunctionInfo(fi));
					if (right == null)
						continue;
					var left = Expression.Field(_pResult, fi);
					_bodyList.Add(Expression.Assign(left, right));
				}

				foreach (var pi in _targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
				{
					if (!pi.CanWrite)
						continue;
					var right = CreateFunction(new FieldFunctionInfo(pi));
					if (right == null)
						continue;
					var left = Expression.Property(_pResult, pi);
					_bodyList.Add(Expression.Assign(left, right));
				}

				if (_supportInitialize)
					CallEndInit();

				_bodyList.Add(Expression.Label(Expression.Label(_targetType), _pResult));

				var delegateType = typeof(Func<,>).MakeGenericType(typeof(ICfgNode), _targetType);

				return Expression.Lambda(delegateType, Expression.Block(new[] { _pResult }, _bodyList), _pCfgNode).Compile();
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(string.Format("can't create a deserialize function for '{0}'", _targetType.FullName), ex);
			}
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
			_configureFieldInfo(ffi);
			return ffi.Ignore ? null : MakeFieldReader(ffi);
		}

		private Expression MakeFieldReader(FieldFunctionInfo ffi)
		{
			switch (ffi.Function)
			{
				case FieldFunctionType.Primitive:
				{
					var mi = ffi.Required ? BuildToolkit.RequiredPrimitiveFieldMI : BuildToolkit.OptionalPrimitiveFieldMI;
					mi = mi.MakeGenericMethod(ffi.ResultType);
					return Expression.Call(null, mi, Expression.Constant(_converter), Expression.Constant(ffi.Name), _pCfgNode);
				}

				case FieldFunctionType.Array:
				{
					var itemType = ffi.ResultType.GetElementType();
					var mi = BuildToolkit.ArrayMI.MakeGenericMethod(itemType);
					return Expression.Call(null, mi, Expression.Constant(ffi.Name), _pCfgNode, Expression.Constant(_deserializer));
				};

				case FieldFunctionType.Collection:
				{
					var itemType = ffi.ResultType.GetGenericArguments()[0];
					var mi = BuildToolkit.ListMI.MakeGenericMethod(itemType);
					return Expression.Call(null, mi, Expression.Constant(ffi.Name), _pCfgNode, Expression.Constant(_deserializer));
				};

				case FieldFunctionType.Complex:
				{
					var mi = ffi.Required ? BuildToolkit.RequiredComplexFieldMI : BuildToolkit.OptionalComplexFieldMI;
					mi = mi.MakeGenericMethod(ffi.ResultType);
					return Expression.Call(null, mi, Expression.Constant(ffi.Name), _pCfgNode, Expression.Constant(_deserializer));
				};

				default:
					throw new InvalidOperationException("unexpected FieldFunctionType");
			}
		}
	}
}
