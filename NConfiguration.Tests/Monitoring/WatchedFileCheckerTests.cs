using System;
using System.IO;
using System.Threading;
using NUnit.Framework;

namespace NConfiguration.Monitoring
{
	[TestFixture]
	public class WatchedFileCheckerTests
	{
		private string _xmlCfg = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
	<WatchFile Mode=""Auto"" />
	<AdditionalConfig F=""Origin"" />
</configuration>";

		[TestCase]
		public void MoveDirectory()
		{
			var tempPath = Path.Combine(Path.GetTempPath(), "NConfiguration.Tests");
			var dirName = Guid.NewGuid().ToString();
			var watchedFile = Path.Combine(tempPath, dirName, "test.xml");

			Directory.CreateDirectory(Path.Combine(tempPath, dirName));

			File.WriteAllText(watchedFile, _xmlCfg);

			var fileInfo = ReadedFileInfo.Create(watchedFile, _ => _.CopyTo(Stream.Null));
			var fileChecker = FileChecker.TryCreate(fileInfo, WatchMode.System, null, CheckMode.All);

			var wait = new ManualResetEvent(false);
			fileChecker.Changed += (a, e) =>
			{
				wait.Set();
			};

			Directory.Move(Path.Combine(tempPath, dirName), Path.Combine(tempPath, dirName + "_moved"));

			Assert.IsTrue(wait.WaitOne(5000), "no event");
			Directory.CreateDirectory(Path.Combine(tempPath, dirName)); //no lock directory
		}

		[TestCase]
		public void DeleteDirectory()
		{
			var tempPath = Path.Combine(Path.GetTempPath(), "NConfiguration.Tests");
			var dirName = Guid.NewGuid().ToString();
			var watchedFile = Path.Combine(tempPath, dirName, "test.xml");

			Directory.CreateDirectory(Path.Combine(tempPath, dirName));

			File.WriteAllText(watchedFile, _xmlCfg);

			var fileInfo = ReadedFileInfo.Create(watchedFile, _ => _.CopyTo(Stream.Null));
			var fileChecker = FileChecker.TryCreate(fileInfo, WatchMode.System, null, CheckMode.All);

			var wait = new ManualResetEvent(false);
			fileChecker.Changed += (a, e) =>
			{
				wait.Set();
			};

			Directory.Delete(Path.Combine(tempPath, dirName), true);

			Assert.IsTrue(wait.WaitOne(5000), "no event");
			Directory.CreateDirectory(Path.Combine(tempPath, dirName)); //no lock directory
		}
	}
}
