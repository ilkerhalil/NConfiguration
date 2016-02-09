using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NConfiguration.Combination
{
	internal static partial class BuildUtils
	{
		private static readonly HashSet<Type> _simplySystemStructs = new HashSet<Type>
			{
				typeof(string),
				typeof(Enum), typeof(DateTime), typeof(DateTimeOffset),
				typeof(bool), typeof(byte), typeof(char), typeof(decimal),
				typeof(double), typeof(Guid), typeof(short), typeof(int),
				typeof(long), typeof(sbyte), typeof(float), typeof(TimeSpan),
				typeof(ushort), typeof(uint), typeof(ulong)
			};

		private static bool IsSimplyStruct(Type type)
		{
			if (type.IsEnum)
				return true;

			var ntype = Nullable.GetUnderlyingType(type);
			if (ntype != null) // is Nullable<>
			{
				type = ntype;
				if (type.IsEnum)
					return true;
			}

			return _simplySystemStructs.Contains(type);
		}

		public static object CreateFunction(Type targetType)
		{
			var combinerAttr = targetType.GetCustomAttribute<CombinerAttribute>(false);
			if (combinerAttr != null)
			{
				//UNDONE combinerAttr.CombinerType maybe generic
				if (typeof(ICombiner<>).MakeGenericType(targetType).IsAssignableFrom(combinerAttr.CombinerType))
				{
					return CreateByCombinerInterfaceMI.MakeGenericMethod(combinerAttr.CombinerType, targetType).Invoke(null, new object[0]);
				}
			}

			if(typeof(ICombinable<>).MakeGenericType(targetType).IsAssignableFrom(targetType))
			{
				var combinerType = (targetType.IsValueType ?
					typeof(GenericStructCombiner<>) :
					typeof(GenericClassCombiner<>)).MakeGenericType(targetType);

				return CreateByCombinerInterfaceMI.MakeGenericMethod(combinerType, targetType).Invoke(null, new object[0]);
			}

			if (typeof(ICombinable).IsAssignableFrom(targetType))
			{
				var combinerType = (targetType.IsValueType ?
					typeof(StructCombiner<>) :
					typeof(ClassCombiner<>)).MakeGenericType(targetType);

				return CreateByCombinerInterfaceMI.MakeGenericMethod(combinerType, targetType).Invoke(null, new object[0]);
			}

			if (IsSimplyStruct(targetType))
			{
				var supressValue = CreateDefaultSupressor(targetType);
				return CreateForwardCombiner(targetType, supressValue);
			}

			//UNDONE
			throw new NotImplementedException();
		}

		internal static readonly MethodInfo CreateByCombinerInterfaceMI = GetMethod("CreateByCombinerInterface");
		internal static Combine<T> CreateByCombinerInterface<C, T>() where C: ICombiner<T>
		{
			var combiner = Activator.CreateInstance<C>();
			return combiner.Combine;
		}

		internal static object CreateForwardCombiner(Type type, object supressValue)
		{
			var funcType = typeof(Combine<>).MakeGenericType(type);


			return Delegate.CreateDelegate(funcType, supressValue, ForwardCombineMI.MakeGenericMethod(type));
		}

		internal static readonly MethodInfo ForwardCombineMI = GetMethod("ForwardCombine");
		internal static T ForwardCombine<T>(Predicate<T> supressValue, ICombiner combiner, T x, T y)
		{
			return supressValue(y) ? x : y;
		}

		private static object CreateDefaultSupressor(Type type)
		{
			var funcType = typeof(Predicate<>).MakeGenericType(type);

			var ntype = Nullable.GetUnderlyingType(type);
			if (ntype != null) // is Nullable<>
				return Delegate.CreateDelegate(funcType, NullableStructSupressMI.MakeGenericMethod(ntype));

			if (type.IsValueType)
				return Delegate.CreateDelegate(funcType, SelectStructSupresssor(type).MakeGenericMethod(type));
			else
				return Delegate.CreateDelegate(funcType, ClassSupressMI.MakeGenericMethod(type));
		}

		internal static readonly MethodInfo NullableStructSupressMI = GetMethod("NullableStructSupress");
		internal static bool NullableStructSupress<T>(T? item) where T : struct
		{
			return item == null;
		}

		internal static readonly MethodInfo ClassSupressMI = GetMethod("ClassSupress");
		internal static bool ClassSupress<T>(T item) where T : class
		{
			return item == null;
		}

		internal static MethodInfo SelectStructSupresssor(Type type)
		{
			var eqInterface = typeof(IEquatable<>).MakeGenericType(type);
			if (eqInterface.IsAssignableFrom(type))
				return EquatableStructSupressMI;

			var comInterface = typeof(IComparable<>).MakeGenericType(type);
			if (comInterface.IsAssignableFrom(type))
				return ComparableStructSupressMI;

			return OthersStructSupressMI;
		}

		internal static readonly MethodInfo EquatableStructSupressMI = GetMethod("EquatableStructSupress");
		internal static bool EquatableStructSupress<T>(T item) where T : struct, IEquatable<T>
		{
			return item.Equals(default(T));
		}

		internal static readonly MethodInfo ComparableStructSupressMI = GetMethod("ComparableStructSupress");
		internal static bool ComparableStructSupress<T>(T item) where T : struct, IComparable<T>
		{
			return item.CompareTo(default(T)) == 0;
		}

		internal static readonly MethodInfo OthersStructSupressMI = GetMethod("OthersStructSupress");
		internal static bool OthersStructSupress<T>(T item) where T : struct
		{
			return item.Equals(default(T));
		}

		private static MethodInfo GetMethod(string name)
		{
			return typeof(BuildUtils).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic);
		}
	}
}
