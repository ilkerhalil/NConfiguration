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
	public class CombineMapper : ICombineMapper
	{
		private readonly HashSet<Type> _simplySystemStructs = new HashSet<Type>
			{
				typeof(string),
				typeof(Enum), typeof(DateTime), typeof(DateTimeOffset),
				typeof(bool), typeof(byte), typeof(char), typeof(decimal),
				typeof(double), typeof(Guid), typeof(short), typeof(int),
				typeof(long), typeof(sbyte), typeof(float), typeof(TimeSpan),
				typeof(ushort), typeof(uint), typeof(ulong)
			};

		public CombineMapper()
		{
		}

		private bool IsSimplyStruct(Type type)
		{
			if (type.IsEnum)
				return true;

			var ntype = Nullable.GetUnderlyingType(type);
			if (ntype != null) // is Nullable<>
				type = ntype;

			return _simplySystemStructs.Contains(type);
		}

		public object CreateFunction(Type targetType, IGenericCombiner combiner)
		{
			if (typeof(ICombinable_obsolete).IsAssignableFrom(targetType))
			{
				var funcType = typeof(Func<,,>).MakeGenericType(targetType, targetType, targetType);

				if (targetType.IsValueType)
					return Delegate.CreateDelegate(funcType, CustomStructCombineMI.MakeGenericMethod(targetType));
				else
					return Delegate.CreateDelegate(funcType, CustomClassCombineMI.MakeGenericMethod(targetType));
			}

			if(IsSimplyStruct(targetType))
				return BuildToolkit.CreateForwardCombiner(targetType);

			object result = null;

			result = BuildToolkit.TryCreateRecursiveNullableCombiner(targetType, combiner);
			if(result != null)
				return result;

			result = BuildToolkit.TryCreateCollectionCombiner(targetType);
			if(result != null)
				return result;

			var builder = new ComplexFunctionBuilder(targetType, combiner);
			result = builder.Compile();
			if(result != null)
				return result;
			
			return BuildToolkit.CreateForwardCombiner(targetType);
		}

		internal static readonly MethodInfo CustomClassCombineMI = typeof(CombineMapper).GetMethod("CustomClassCombine", BindingFlags.Static | BindingFlags.NonPublic);

		internal static T CustomClassCombine<T>(T x, T y) where T : class, NConfiguration.ICombinable_obsolete
		{
			if (x == null)
				return y;

			x.Combine(y);
			return x;
		}

		internal static readonly MethodInfo CustomStructCombineMI = typeof(CombineMapper).GetMethod("CustomStructCombine", BindingFlags.Static | BindingFlags.NonPublic);

		internal static T CustomStructCombine<T>(T x, T y) where T : struct, NConfiguration.ICombinable_obsolete
		{
			x.Combine(y);
			return x;
		}

	}
}
