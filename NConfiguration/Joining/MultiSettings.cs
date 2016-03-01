using System;
using System.Linq;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Threading;
using NConfiguration.Serialization;
using NConfiguration.Combination;

namespace NConfiguration.Joining
{
	public class MultiSettings : IAppSettings, IChangeable
	{
		private readonly object _sync = new object();
		private bool _changed = false;
		private object _firstChangedSource = null;

		internal MultiSettings()
		{
		}

		internal void Observe(IChangeable changable)
		{
			if(changable != null)
				changable.Changed += OnInnerChangableChanged;
		}

		private void OnInnerChangableChanged(object sender, EventArgs e)
		{
			EventHandler copy;
			lock (_sync)
			{
				if (_changed)
					return;

				_changed = true;
				copy = _changedHandler;
				_changedHandler = null;
				_firstChangedSource = sender;
			}

			if (copy != null)
				copy(_firstChangedSource, EventArgs.Empty);
		}

		private EventHandler _changedHandler = null;

		/// <summary>
		/// Instance changed.
		/// </summary>
		public event EventHandler Changed
		{
			add
			{
				lock (_sync)
				{
					if (_changed)
						ThreadPool.QueueUserWorkItem(AsyncChangedWork, value);
					else
						_changedHandler += value;
				}
			}
			remove
			{
				lock (_sync)
				{
					if (!_changed)
						_changedHandler -= value;
				}
			}
		}

		private void AsyncChangedWork(object arg)
		{
			((EventHandler)arg)(_firstChangedSource, EventArgs.Empty);
		}

		public IConfigNodeProvider Nodes { get; internal set; }

		public IDeserializer Deserializer { get; internal set; }

		public ICombiner Combiner { get; internal set; }
	}
}

