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

		private static Type GetEnumerableType(Type type)
		{
			foreach (Type intType in type.GetInterfaces())
			{
				if (intType.IsGenericType
					&& intType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
				{
					return intType.GetGenericArguments()[0];
				}
			}
			return null;
		}

		public static object CreateFunction(Type targetType)
		{
			var supressValue = CreateDefaultSupressor(targetType);

			return
				TryCreateAsCombinable(targetType) ??
				TryCreateAsAttribute(targetType) ??
				TryCreateForSimplyStruct(targetType, supressValue) ??
				TryCreateRecursiveNullableCombiner(targetType, supressValue) ??
				TryCreateCollectionCombiner(targetType) ??
				TryCreateComplexCombiner(targetType) ??
				CreateForwardCombiner(targetType, supressValue);
		}

		private static object TryCreateCollectionCombiner(Type targetType)
		{
			var itemType = GetEnumerableType(targetType);
			if (itemType == null)
				return null;

			var funcType = typeof(Combine<>).MakeGenericType(targetType);

			return Delegate.CreateDelegate(funcType, CollectionCombineMI.MakeGenericMethod(targetType, itemType));
		}

		private static object TryCreateComplexCombiner(Type targetType)
		{
			var builder = new ComplexFunctionBuilder(targetType);

			foreach (var fi in targetType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
				builder.Add(fi);

			foreach (var pi in targetType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
				builder.Add(pi);

			return builder.Compile();
		}

		internal static object TryCreateAsAttribute(Type targetType)
		{
			var combinerAttr = targetType.GetCustomAttributes(false).OfType<ICombinerFactory>().SingleOrDefault();
			
			if(combinerAttr == null)
				return null;
			
			var combiner = combinerAttr.CreateInstance(targetType);
			
			var mi = typeof(ICombiner<>).MakeGenericType(targetType).GetMethod("Combine");
			var funcType = typeof(Combine<>).MakeGenericType(targetType);
			return Delegate.CreateDelegate(funcType, combiner, mi);
		}

		internal static object TryCreateAsCombinable(Type targetType)
		{
			Type combinerType;
			if (typeof(ICombinable<>).MakeGenericType(targetType).IsAssignableFrom(targetType))
				combinerType = targetType.IsValueType ? typeof(GenericStructCombiner<>) : typeof(GenericClassCombiner<>);
			else if (typeof(ICombinable).IsAssignableFrom(targetType))
				combinerType = targetType.IsValueType ? typeof(StructCombiner<>) : typeof(ClassCombiner<>);
			else
				return null;

			combinerType = combinerType.MakeGenericType(targetType);
			return CreateByCombinerInterfaceMI.MakeGenericMethod(combinerType, targetType).Invoke(null, new object[0]);
		}

		internal static object TryCreateForSimplyStruct(Type targetType, object supressValue)
		{
			if (IsSimplyStruct(targetType))
			{
				return CreateForwardCombiner(targetType, supressValue);
			}
			else
				return null;
		}

		internal static object TryCreateRecursiveNullableCombiner(Type targetType, object supressValue)
		{
			var ntype = Nullable.GetUnderlyingType(targetType);
			if (ntype == null) // is not Nullable<>
				return null;

			var funcType = typeof(Combine<>).MakeGenericType(targetType);

			return Delegate.CreateDelegate(funcType, supressValue, RecursiveNullableCombineMI.MakeGenericMethod(ntype));
		}

		internal static readonly MethodInfo CollectionCombineMI = GetMethod("CollectionCombine");
		internal static T CollectionCombine<T, I>(ICombiner combiner, T x, T y) where T: IEnumerable<I>
		{
			if (x == null)
				return y;

			if (y == null)
				return x;

			if (!x.Any())
				return y;

			if (!y.Any())
				return x;

			return y;
		}

		internal static readonly MethodInfo RecursiveNullableCombineMI = GetMethod("RecursiveNullableCombine");
		internal static T? RecursiveNullableCombine<T>(Predicate<T?> supressValue, ICombiner combiner, T? x, T? y) where T : struct
		{
			if (supressValue(x)) return y;
			if (supressValue(y)) return x;
			return combiner.Combine<T>(x.Value, y.Value);
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
