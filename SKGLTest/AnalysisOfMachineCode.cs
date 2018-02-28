using System;
using System.Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SKGL;

namespace SKGLTest
{
	[TestClass]
	public class AnalysisOfMachineCode
	{
		public void TestGetMachineCode()
		{
			// Arrange
			// Act
			var code = MachineCodeManager.GetMachineCode();

			// Assert
			Assert.IsInstanceOfType(code, typeof(int));
			Assert.IsTrue(code <= 100000);
			Console.WriteLine($"TestGetMachineCode: MachineCode = {code}");
		}

		[TestMethod]
		public void TestGetProcessorId()
		{
			// Arrange
			var searcher = new ManagementObjectSearcher();
			var info = string.Empty;

			// Act
			info = MachineCodeManager.GetProcessorId(searcher, info);

			// Assert
			Assert.IsFalse(string.IsNullOrWhiteSpace(info));
			Console.WriteLine($"TestGetProcessorId: Processor id = {info}");
		}

		[TestMethod]
		public void TestGetBIOSId()
		{
			// Arrange
			var searcher = new ManagementObjectSearcher();
			var info = string.Empty;

			// Act
			info = MachineCodeManager.GetBIOSSerialNumber(searcher, info);

			// Assert
			Assert.IsFalse(string.IsNullOrWhiteSpace(info));
			Console.WriteLine($"TestGetBIOSId: BIOS id = {info}");
		}

		[TestMethod]
		public void TestGetBaseboardId()
		{
			// Arrange
			var searcher = new ManagementObjectSearcher();
			var info = string.Empty;

			// Act
			info = MachineCodeManager.GetBaseboardSerialNumber(searcher, info);

			// Assert
			Assert.IsFalse(string.IsNullOrWhiteSpace(info));
			Console.WriteLine($"TestGetBaseboardId: Baseboard SerialNumber = {info}");
		}


		[TestMethod]
		public void TestGetHddSerialNumber()
		{
			// Arrange
			// Act
			var info = MachineCodeManager.GetHddSerialNumber();

			// Assert
			Assert.IsFalse(string.IsNullOrWhiteSpace(info));
			Console.WriteLine($"TestGetHddSerialNumber: Hdd SerialNumber = {info}");
		}

		[TestMethod]
		// ReSharper disable once InconsistentNaming
		public void TestGetNIC()
		{
			// Arrange
			// Act
			var info = MachineCodeManager.GetHddSerialNumber();

			// Assert
			Assert.IsFalse(string.IsNullOrWhiteSpace(info));
			Console.WriteLine($"TestGetHddSerialNumber: Hdd SerialNumber = {info}");
		}
	}
}
