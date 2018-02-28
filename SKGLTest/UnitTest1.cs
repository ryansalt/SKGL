using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SKGLTest
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void MachineCodeTest()
		{
			var gen = new SKGL.Generate();
			var a = gen.MachineCode.ToString();
			Assert.IsNotNull(a);
			Console.WriteLine($"MachineCode: {a}");
		}

		[TestMethod]
		public void CreateAndValidateSimple()
		{
			// Arrange
			var gen = new SKGL.Generate();
			var a = gen.DoKey(30);

			// Act
			var val = new SKGL.Validate {Key = a};

			// Assert
			Assert.IsTrue(val.IsValid);
			Assert.IsTrue(val.IsExpired == false);
			Assert.IsTrue(val.SetTime == 30);

		}

		[TestMethod]
		public void CreateAndValidateA()
		{
			// Arrange
			const string key = "MXNBF-ITLDZ-WPOBY-UCHQW";
			const string secretPhrase = "567";

			// Act
			var val = new SKGL.Validate
			{
				Key = key,
				SecretPhrase = secretPhrase
			};
			
			// Assert
			Assert.IsTrue(val.IsValid);
			Assert.IsTrue(val.IsExpired);
			Assert.IsTrue(val.SetTime == 30);

		}

		[TestMethod]
		public void CreateAndValidateC()
		{
			var skm = new SKGL.SerialKeyConfiguration();

			var gen = new SKGL.Generate(skm);
			skm.Features[0] = true;
			gen.SecretPhrase = "567";
			var a = gen.DoKey(37);


			var val = new SKGL.Validate
			{
				Key = a,
				SecretPhrase = "567"
			};


			Assert.IsTrue(val.IsValid);
			Assert.IsTrue(val.IsExpired == false);
			Assert.IsTrue(val.SetTime == 37);
			Assert.IsTrue(val.Features[0]);
			Assert.IsTrue(val.Features[1] == false);

		}


		[TestMethod]
		// ReSharper disable once InconsistentNaming
		public void CreateAndValidateCJ()
		{
			var val = new SKGL.Validate
			{
				Key = "LZWXQ-SMBAS-JDVDL-XTEHB",
				SecretPhrase = "567"
			};
			Assert.IsTrue(val.IsValid);
			Assert.IsTrue(val.IsExpired);
			Assert.IsTrue(val.SetTime == 30);
			Assert.IsTrue(val.Features[0]);
		}

		[TestMethod]
		// ReSharper disable once InconsistentNaming
		public void CreateAndValidateAM()
		{
			var gen = new SKGL.Generate();
			var a = gen.DoKey(30);

			var validateAKey = new SKGL.Validate {Key = a};


			Assert.IsTrue(validateAKey.IsValid);
			Assert.IsTrue(validateAKey.IsExpired == false);
			Assert.IsTrue(validateAKey.SetTime == 30);

			if (validateAKey.IsValid)
			{
				// displaying date
				// remember to use .ToShortDateString after each date!
				Console.WriteLine("This key is created {0}", validateAKey.CreationDate.ToShortDateString());
				Console.WriteLine("This key will expire {0}", validateAKey.ExpireDate.ToShortDateString());

				Console.WriteLine("This key is set to be valid in {0} day(s)", validateAKey.SetTime);
				Console.WriteLine("This key has {0} day(s) left", validateAKey.DaysLeft);

			}
			else
			{
				// if invalid
				Console.WriteLine("Invalid!");
			}

		}
	}
}
