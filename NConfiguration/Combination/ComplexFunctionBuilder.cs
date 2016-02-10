using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace NConfiguration.Combination
{
	public class ComplexFunctionBuilder
	{
		private Type _targetType;
		private IGenericCombiner _combiner;
		private List<Expression> _bodyList = new List<Expression>();
		private ParameterExpression _pPrev;
		private ParameterExpression _pNext;

		public ComplexFunctionBuilder(Type targetType, IGenericCombiner combiner)
		{
			_targetType = targetType;
			_pPrev = Expression.Parameter(_targetType);
			_pNext = Expression.Parameter(_targetType);
			_combiner = combiner;
		}

		public object Compile()
		{
			try
			{
				LabelTarget returnTarget = Expression.Label(_targetType);

				if (!_targetType.IsValueType)
				{
					_bodyList.Add(Expression.IfThen(Expression.Equal(_pNext, Expression.Constant(null)), Expression.Return(returnTarget, _pPrev)));
					_bodyList.Add(Expression.IfThen(Expression.Equal(_pPrev, Expression.Constant(null)), Expression.Return(returnTarget, _pNext)));
				}

				bool assingExist = false;

				foreach (var fi in _targetType.GetFields(BindingFlags.Instance | BindingFlags.Public))
				{
					var prevField = Expression.Field(_pPrev, fi);
					var nextField = Expression.Field(_pNext, fi);
					var right = CreateFunction(fi.FieldType, fi, prevField, nextField);
					if (right == null)
						continue;
					_bodyList.Add(Expression.Assign(prevField, right));
					assingExist = true;
				}

				foreach (var pi in _targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
				{
					if (!pi.CanWrite || !pi.CanRead)
						continue;

					var prevProp = Expression.Property(_pPrev, pi);
					var nextProp = Expression.Property(_pNext, pi);
					var right = CreateFunction(pi.PropertyType, pi, prevProp, nextProp);
					if (right == null)
						continue;
					_bodyList.Add(Expression.Assign(prevProp, right));
					assingExist = true;
				}

				if (!assingExist)
					return null;

				_bodyList.Add(Expression.Return(returnTarget, _pPrev));
				_bodyList.Add(Expression.Label(returnTarget, _pPrev));

				var delegateType = typeof(Func<,,>).MakeGenericType(_targetType, _targetType, _targetType);

				return Expression.Lambda(delegateType, Expression.Block(_bodyList), _pPrev, _pNext).Compile();
			}
			catch (Exception ex)
			{
				throw new InvalidOperationException(string.Format("can't create a combine function for '{0}'", _targetType.FullName), ex);
			}
		}

		private Expression CreateFunction(Type fieldType, MemberInfo memberInfo, Expression prev, Expression next)
		{
			if (memberInfo.GetCustomAttributes(typeof(IgnoreDataMemberAttribute), true).Length != 0)
				return null;
			if (memberInfo.GetCustomAttributes(typeof(XmlIgnoreAttribute), true).Length != 0)
				return null;

			var mi = BuildToolkit.FieldCombineMI;
			mi = mi.MakeGenericMethod(fieldType);
			return Expression.Call(null, mi, Expression.Constant(_combiner), prev, next);
		}
	}
}
