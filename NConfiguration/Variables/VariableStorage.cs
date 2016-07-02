using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using NConfiguration.Monitoring;
using NConfiguration.Serialization;

namespace NConfiguration.Variables
{
	public class VariableStorage
	{
		private readonly Dictionary<string, string> _map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		public void Set(string name, string value)
		{
			if (_map.ContainsKey(name))
				return;

			_map.Add(name, value);
		}

		public string Get(string name)
		{
			string value;
			if (_map.TryGetValue(name, out value))
				return value;

			throw new InvalidOperationException(string.Format("variable '{0}' not found", name));
		}

		public ICfgNode CfgNodeConverter(string name, ICfgNode candidate)
		{
			if (NameComparer.Equals(name, "variable"))
			{
				var varConfig = DefaultDeserializer.Instance.Deserialize<VariableConfig>(candidate);
				Set(varConfig.Name, varConfig.Value);
				return null;
			}

			return new CfgNodeWrapper(candidate, this);
		}

		internal class VariableConfig
		{
			[DataMember(Name = "Name", IsRequired = true)]
			public string Name { get; set; }

			[DataMember(Name = "Value", IsRequired = true)]
			public string Value { get; set; }
		}
	}
}
