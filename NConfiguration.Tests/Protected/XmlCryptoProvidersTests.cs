using System;
using System.Linq;
using System.Security.Cryptography;
using System.Xml;
using NUnit.Framework;
using System.Configuration;
using NConfiguration.Xml.Protected;
using System.Collections.Generic;

namespace NConfiguration
{
	[TestFixture]
	public class XmlCryptoProvidersTests
	{
		[Test]
		public void LoadProviderSettingsFromConfigProtectedData()
		{
			var providerSettings = ProviderLoader.ConfigProtectedDataProviders.ToArray();
			Assert.AreEqual(2, providerSettings.Length);
			Assert.AreEqual("RsaProvider", providerSettings[0].Name);
			Assert.AreEqual("DpapiProvider", providerSettings[1].Name);
		}

		[Test]
		public void LoadFromConfigProtectedData()
		{
			var providers = ProviderLoader.FromConfigProtectedData().Providers;
			
			Assert.IsNotNull(providers.Get("RsaProvider"));
			Assert.IsNotNull(providers.Get("DpapiProvider"));
		}


		[Test]
		public void ProviderFiltering()
		{
			var settings = @"<Config>
<configProtectedData>
	<providers>
		<add name='ClearingProvider' type='System.Configuration.RsaProtectedConfigurationProvider, System.Configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' keyContainerName='SampleKeys' useMachineContainer='true' />
		<clear/>
		<add name='RsaProvider' type='System.Configuration.RsaProtectedConfigurationProvider, System.Configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' keyContainerName='SampleKeys' useMachineContainer='true' />
		<add name='MissedProvider' type='System.Configuration.DpapiProtectedConfigurationProvider, System.Configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' />
		<add name='DpapiProvider' type='System.Configuration.DpapiProtectedConfigurationProvider, System.Configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' />
	</providers>
</configProtectedData>
</Config>".ToXmlSettings().AsSingleSettings();

			var names = new List<string>();

			var providers = new ProviderLoader()
				.SubscribeLoading((s, e) =>
				{
					names.Add(e.Name);
					if (e.Name == "MissedProvider")
						e.Canceled = true;
				})
				.LoadAppSettings(settings)
				.Providers;

			CollectionAssert.AreEqual(new string[] { "ClearingProvider", "RsaProvider", "MissedProvider", "DpapiProvider" }, names);
			Assert.IsNull(providers.Get("MissedProvider"));
			Assert.IsNull(providers.Get("ClearingProvider"));
			Assert.IsNotNull(providers.Get("RsaProvider"));
			Assert.IsNotNull(providers.Get("DpapiProvider"));
		}

		void loader_ProviderLoading(object sender, ProviderLoadingEventArgs e)
		{
			throw new NotImplementedException();
		}
		
		[Test]
		public void LoadFromAppSettings()
		{
			var settings = @"<Config>
<configProtectedData defaultProvider='SampleProvider'>
	<providers>
		<clear/>
		<add name='RsaProvider'
			type='System.Configuration.RsaProtectedConfigurationProvider, System.Configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
			keyContainerName='SampleKeys' useMachineContainer='true' />
		<add name='DpapiProvider' type='System.Configuration.DpapiProtectedConfigurationProvider, System.Configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a' />
	</providers>
</configProtectedData>
</Config>".ToXmlSettings().AsSingleSettings();

			var providers = ProviderLoader.FromAppSettings(settings).Providers;
			
			Assert.IsNotNull(providers.Get("RsaProvider"));
			Assert.IsNotNull(providers.Get("DpapiProvider"));
		}
		
		private const string CryptoProvidersSettings = @"
<Config>
	<configProtectedData>
		<providers>
			<add name='RsaTestProvider'
				type='System.Configuration.RsaProtectedConfigurationProvider, System.Configuration, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'
				keyContainerName='MyTestKeys' useMachineContainer='true' />
		</providers>
	</configProtectedData>
</Config>";
		
		private const string EncryptedCfg = @"
<Config>
	<MyXmlCfg configProtectionProvider='RsaTestProvider'>
		<EncryptedData Type='http://www.w3.org/2001/04/xmlenc#Element' xmlns='http://www.w3.org/2001/04/xmlenc#'>
			<EncryptionMethod Algorithm='http://www.w3.org/2001/04/xmlenc#aes256-cbc' />
			<KeyInfo xmlns='http://www.w3.org/2000/09/xmldsig#'>
				<EncryptedKey xmlns='http://www.w3.org/2001/04/xmlenc#'>
					<EncryptionMethod Algorithm='http://www.w3.org/2001/04/xmlenc#rsa-1_5' />
					<KeyInfo xmlns='http://www.w3.org/2000/09/xmldsig#'>
						<KeyName>Rsa Key</KeyName>
					</KeyInfo>
					<CipherData>
						<CipherValue>
bm79LAyiP68B2YTs/CIL35Hpw6uVWvrs93Rm0XBVeFVXcZkmldgNpryUiJ6pnYWbVfAGWKe8tNAUkTq
hXNRC/C6rnBTXQgmgF4bh7JrIvWuWQBzV6ahKfAsuHTvSOMBlgYivNiGlXZdTCD6SaGorKa0zPyoELC
38jlEjaBoGLwuKzVW30JnVWMDtOuNb84I+t4UyYfhiMQHL1rGXgD6bZ66c/ulkhuiJ1m0GIi9/x3WqR
92bL7eFg6E3ZbD3eNOcgMAJjQ8DrrPFc12zwF0Bwolhs1/3aV4tP6rlTCTIdkR8adWlyWjFLSDiNiNK
D7/WOjEvRO0Bjoy9Ykc7nwrHgQ==
						</CipherValue>
					</CipherData>
				</EncryptedKey>
			</KeyInfo>
			<CipherData>
				<CipherValue>
rudDpM9BHMQ29cQ6Vbcsf7aOvCs+6QPg5lPxQvP/qWao70BgZOFrTCiOXEEnnoidTjuahy50eHdTVc9
iaWGZaw==
				</CipherValue>
			</CipherData>
		</EncryptedData>
	</MyXmlCfg>
</Config>
";

		[Test]
		public void DecryptSection()
		{
			var settings = CryptoProvidersSettings.ToXmlSettings().AsSingleSettings();

			KeyManager.Create();

			var providers = ProviderLoader.FromAppSettings(settings).Providers;
			var cryptedsettings = EncryptedCfg.ToXmlSettings(providers).AsSingleSettings();

			var cfg = cryptedsettings.First<MyXmlConfig>();

			Assert.AreEqual("SecureMessage", cfg.AttrField);
			Assert.IsNull(cfg.ElemField);

			KeyManager.Delete();
		}
		
		[Test]
		public void DecryptFail()
		{
			KeyManager.Delete();
			
			var settings = CryptoProvidersSettings.ToXmlSettings().AsSingleSettings();
			var providers = ProviderLoader.FromAppSettings(settings).Providers;
			var cryptedsettings = EncryptedCfg.ToXmlSettings(providers).AsSingleSettings();

			try
			{
				cryptedsettings.First<MyXmlConfig>();
				Assert.Fail("expected exception");
			}
			catch(DeserializeChildException ex)
			{
				Assert.IsInstanceOf<CryptographicException>(ex.InnerException);
			}
		}

		[Test]
		public void EncryptSection()
		{
			var settings = CryptoProvidersSettings.ToXmlSettings().AsSingleSettings();

			KeyManager.Create();

			var providers = ProviderLoader.FromAppSettings(settings).Providers;

			var provider = providers.Get("RsaTestProvider");

			var el = "<MyXmlCfg AttrField='SecureMessage'/>".ToXmlElement();
			
			var encryptedXml = provider.Encrypt(el).OuterXml;

			Assert.IsFalse(encryptedXml.Contains("SecureMessage"));
			Assert.IsTrue(encryptedXml.Contains("EncryptedData"));
			
			providers = ProviderLoader.FromAppSettings(settings).Providers;
			var cryptedsettings = string.Format(
				@"<Config><MyXmlCfg configProtectionProvider='RsaTestProvider'>{0}</MyXmlCfg></Config>",
				encryptedXml
			).ToXmlSettings(providers).AsSingleSettings();


			var cfg = cryptedsettings.First<MyXmlConfig>();

			Assert.AreEqual("SecureMessage", cfg.AttrField);
			Assert.IsNull(cfg.ElemField);

			KeyManager.Delete();
		}
	}
}

