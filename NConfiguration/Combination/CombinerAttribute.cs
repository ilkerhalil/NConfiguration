using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NConfiguration.Combination
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class CombinerAttribute : Attribute, ICombinerFactory
	{
		public readonly IReadOnlyCollection<Type> CombinerTypes;

		public CombinerAttribute(params Type[] combinerTypes)
		{
			if (combinerTypes == null)
				throw new ArgumentNullException("combinerType");
			CombinerTypes = combinerTypes;
		}

		public virtual object CreateInstance(Type targetType)
		{
			if (targetType == null)
				throw new ArgumentNullException("targetType");

			foreach (var candidate in CombinerTypes)
			{
				Type combinerType;
				try
				{
					combinerType = candidate.MakeGenericType(targetType);
				}
				catch (InvalidOperationException)
				{
					combinerType = candidate;
				}

				if (!typeof(ICombiner<>).MakeGenericType(targetType).IsAssignableFrom(combinerType))
					continue;

				return Activator.CreateInstance(combinerType);
			}

			throw new InvalidOperationException("supported combiner not found");
		}
	}
}
