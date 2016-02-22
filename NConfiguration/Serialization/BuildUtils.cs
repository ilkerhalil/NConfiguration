using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NConfiguration.Serialization
{
	internal static class BuildUtils
	{
		internal static object CreateFunction(Type targetType)
		{
			return
				TryCreateNativeFunction(targetType) ??
				TryCreateAsAttribute(targetType) ??
				SimpleTypes.Converter.TryCreateFunction(targetType) ??
				TryCreateForComplexType(targetType);
		}

		public static bool IsCollection(Type type)
		{
			if (!type.IsGenericType)
				return false;

			var genType = type.GetGenericTypeDefinition();

			return genType == typeof(List<>)
				|| genType == typeof(IList<>)
				|| genType == typeof(ICollection<>)
				|| genType == typeof(IEnumerable<>);
		}

		private static object TryCreateForComplexType(Type targetType)
		{
			try
			{
				var builder = new ComplexFunctionBuilder(targetType);

				foreach (var fi in targetType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
					builder.Add(fi);

				foreach (var pi in targetType.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
					builder.Add(pi);

				return builder.Compile();
			}
			catch(Exception ex)
			{
				throw new InvalidOperationException(string.Format("can't create a deserialize function for '{0}'", targetType.FullName), ex);
			}
		}

		private static object TryCreateAsAttribute(Type targetType)
		{
			var deserializeAttr = targetType.GetCustomAttributes(false).OfType<IDeserializerFactory>().SingleOrDefault();

			if (deserializeAttr == null)
				return null;

			var deserializer = deserializeAttr.CreateInstance(targetType);

			var mi = typeof(IDeserializer<>).MakeGenericType(targetType).GetMethod("Deserialize");
			var funcType = typeof(Deserialize<>).MakeGenericType(targetType);
			return Delegate.CreateDelegate(funcType, deserializer, mi);
		}

		private static object TryCreateNativeFunction(Type targetType)
		{
			if (targetType != typeof(ICfgNode))
				return null;

			var funcType = typeof(Deserialize<>).MakeGenericType(typeof(ICfgNode));
			return Delegate.CreateDelegate(funcType, GetCfgNodeMI);
		}

		internal static readonly MethodInfo GetCfgNodeMI = GetMethod("GetCfgNode");
		internal static ICfgNode GetCfgNode(IDeserializer context, ICfgNode node)
		{
			return node;
		}

		internal static readonly MethodInfo CustomFieldMI = GetMethod("CustomField");
		internal static T CustomField<T>(IDeserializer context, IDeserializer<T> custom, string name, ICfgNode node, bool required)
		{
			try
			{
				var field = node.NestedByName(name).FirstOrDefault();
				if (field == null)
				{
					if (required)
						throw new FormatException("field not found");
					else
						return default(T);
				}

				return custom.Deserialize(context, field);
			}
			catch (Exception ex)
			{
				throw new DeserializeChildException(name, ex);
			}
		}

		internal static readonly MethodInfo ReadFieldMI = GetMethod("ReadField");
		internal static T ReadField<T>(IDeserializer context, string name, ICfgNode node, bool required)
		{
			try
			{
				var field = node.NestedByName(name).FirstOrDefault();
				if (field == null)
				{
					if (required)
						throw new FormatException("field not found");
					else
						return default(T);
				}

				return context.Deserialize<T>(context, field);
			}
			catch (Exception ex)
			{
				throw new DeserializeChildException(name, ex);
			}
		}

		internal static readonly MethodInfo ToListMI = GetMethod("ToList");
		internal static List<T> ToList<T>(IDeserializer context, string name, ICfgNode node)
		{
			int i = 0;
			try
			{
				List<T> result = new List<T>();
				foreach (var chItem in node.NestedByName(name))
				{
					result.Add(context.Deserialize<T>(context, chItem));
					i++;
				}

				return result;
			}
			catch (Exception ex)
			{
				throw new DeserializeChildException(string.Format("{0}[{1}]", name, i), ex);
			}
		}

		internal static readonly MethodInfo ToArrayMI = GetMethod("ToArray");
		internal static T[] ToArray<T>(IDeserializer context, string name, ICfgNode node)
		{
			return ToList<T>(context, name, node).ToArray();
		}

		private static MethodInfo GetMethod(string name)
		{
			return typeof(BuildUtils).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic);
		}
	}
}
