using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Diagnostics;
using System.IO;

namespace RsaToolkit
{
	public class BaseTest
	{
		private string _safeCurrentDirectory;

		[OneTimeSetUp]
		public void SetUp()
		{
			var testsDir = Path.GetDirectoryName(GetType().Assembly.Location);
			var tmpDir = Path.Combine(testsDir, "tmp");
			if(!Directory.Exists(tmpDir))
				Directory.CreateDirectory(tmpDir);

			_safeCurrentDirectory = Environment.CurrentDirectory;
			Environment.CurrentDirectory = tmpDir;
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			Environment.CurrentDirectory = _safeCurrentDirectory;
		}
	}
}
