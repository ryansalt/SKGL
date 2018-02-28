//Copyright (C) 2011-2012 Artem Los, www.clizware.net.
//The author of this code shall get the credits

// This project uses two general algorithms:
//  - Artem's Information Storage Format (Artem's ISF-2)
//  - Artem's Serial Key Algorithm (Artem's SKA-2)

// A great thank to Iberna (https://www.codeplex.com/site/users/view/lberna)
// for getHardDiskSerial algorithm.

using System;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Security;
#if DEBUG
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SKGLTest")]
#endif
[assembly: AllowPartiallyTrustedCallers]
namespace SKGL
{
    internal static class MachineCodeManager
    {

		/// <summary>
		/// Gets a machine code as a hash of ProcessorId, BIOS id, Baseboard ID and NIC.
		/// This code will generate a 5 digits long key, finger print, of the system
		/// where this method is being executed. However, that might be changed in the
		/// hash function "GetStableHash", by changing the amount of zeroes in
		/// MUST_BE_LESS_OR_EQUAL_TO to the one you want to have. Ex 1000 will return 
		/// 3 digits long hash.
		/// </summary>
		/// <returns>System.Int32.</returns>
		/// <remarks>Will only work on Win32</remarks>
		[SecuritySafeCritical]
		internal static int GetMachineCode()
		{
			// please see https://skgl.codeplex.com/workitem/2246 for a list of developers of this code.

			var m = new Methods();

			var searcher = new ManagementObjectSearcher();
			var collectedInfo = string.Empty;

			collectedInfo = GetProcessorId(searcher, collectedInfo);

			collectedInfo = GetBIOSSerialNumber(searcher, collectedInfo);

			collectedInfo = GetBaseboardSerialNumber(searcher, collectedInfo);

			// patch luca bernardini
			if (string.IsNullOrEmpty(collectedInfo) | collectedInfo == "00" | collectedInfo.Length <= 3)
			{
				collectedInfo += GetHddSerialNumber();
			}

			// In case we have message "To be filled by O.E.M." - there is incorrect motherboard/BIOS serial number 
			// - we should relay to NIC
			if (collectedInfo.Contains(Constants.EmptyBiosSerialNumber))
			{
				var nic = GetNicInfo();
				if (!string.IsNullOrWhiteSpace(nic))
					collectedInfo += nic;
			}

			return m.GetEightByteHash(collectedInfo, 100000);

		}

		/// <summary>
		/// Gets the baseboard serial number.
		/// </summary>
		/// <param name="searcher">The searcher.</param>
		/// <param name="collectedInfo">The collected information.</param>
		/// <returns>System.String.</returns>
		/// <remarks>Will only work on Win32</remarks>
		[SecuritySafeCritical]
		internal static string GetBaseboardSerialNumber(ManagementObjectSearcher searcher, string collectedInfo)
		{
			searcher.Query = new ObjectQuery(Constants.Win32BaseboardQuery);
			foreach (var o in searcher.Get())
			{
				var share = o as ManagementObject;
				//finally, the serial number of motherboard
				collectedInfo += share?.GetPropertyValue("SerialNumber");
			}
			return collectedInfo;
		}

		/// <summary>
		/// Gets the bios serial number via ManagementObjectSearcher.
		/// </summary>
		/// <param name="searcher">The searcher.</param>
		/// <param name="collectedInfo">The collected information.</param>
		/// <returns>System.String.</returns>
		/// <remarks>Will only work on Win32</remarks>
		[SecuritySafeCritical]
		internal static string GetBIOSSerialNumber(ManagementObjectSearcher searcher, string collectedInfo)
		{
			searcher.Query = new ObjectQuery(Constants.Win32BiosQuery);
			foreach (var o in searcher.Get())
			{
				var share = o as ManagementObject;
				//then, the serial number of BIOS
				collectedInfo += share?.GetPropertyValue("SerialNumber");
			}
			return collectedInfo;
		}

		/// <summary>
		/// Gets the processor id via ManagementObjectSearcher.
		/// </summary>
		/// <param name="searcher">The searcher.</param>
		/// <param name="collectedInfo"></param>
		/// <returns>System.String.</returns>
		/// <remarks>Will only work on Win32</remarks>
		[SecuritySafeCritical]
		internal static string GetProcessorId(ManagementObjectSearcher searcher, string collectedInfo)
		{
			searcher.Query = new ObjectQuery(Constants.Win32ProcessorQuery);
			// here we will put the informa
			foreach (var o in searcher.Get())
			{
				var share = o as ManagementObject;
				// first of all, the processorid
				collectedInfo += share?.GetPropertyValue("ProcessorId");
			}
			return collectedInfo;
		}

		/// <summary>
		/// Enumerate all Nic adapters, take first one, who has MAC address and return it.
		/// </summary>
		/// <remarks> Function MUST! be updated to select only real NIC cards (and filter out USB and PPTP etc interfaces).
		/// Otherwise user can run in this scenario: a) Insert USB NIC b) Generate machine code c) Remove USB NIC...
		/// </remarks>
		/// <returns>MAC address of NIC adapter</returns>
		/// <remarks>Will only work on Win32</remarks>
		[SecuritySafeCritical]
		internal static string GetNicInfo()
		{
			var nics = NetworkInterface.GetAllNetworkInterfaces();
			string[] mac = { string.Empty };
			foreach (var adapter in nics.Where(adapter => string.IsNullOrWhiteSpace(mac[0])))
			{
				mac[0] = adapter.GetPhysicalAddress().ToString();
			}
			return mac[0];
		}

		/// <summary>
		/// Read the serial number from the hard disk that keep the bootable partition (boot disk)
		/// </summary>
		/// <returns>
		/// If succedes, returns the string rappresenting the Serial Number.
		/// String.Empty if it fails.
		/// </returns>
		[SecuritySafeCritical]
		internal static string GetHddSerialNumber()
		{
			// --- Win32 Disk 
			var searcher = new ManagementObjectSearcher("\\root\\cimv2", Constants.Win32BootPartitionQuery);

			uint diskIndex = 999;
			foreach (var o in searcher.Get())
			{
				var partition = o as ManagementObject;
				diskIndex = Convert.ToUInt32(partition?.GetPropertyValue("DiskIndex")); // should be DiskIndex
				break; // TODO: might not be correct. Was : Exit For
			}

			// I haven't found the bootable partition. Fail.
			if (diskIndex == 999)
				return string.Empty;

			// --- Win32 Disk Drive
			searcher = new ManagementObjectSearcher(string.Format(Constants.Win32DriveQuery, diskIndex));

			var deviceName = string.Empty;
			foreach (var o in searcher.Get())
			{
				if (o is ManagementObject wmiHd)
				{
					deviceName = wmiHd.GetPropertyValue("Name").ToString();
				}
				break; // TODO: might not be correct. Was : Exit For
			}

			// I haven't found the disk drive. Fail
			if (string.IsNullOrWhiteSpace(deviceName))
				return string.Empty;

			// -- Some problems in query parsing with backslash. Using like operator
			if (deviceName != null && deviceName.StartsWith("\\\\.\\"))
			{
				deviceName = deviceName.Replace("\\\\.\\", "%");
			}
			
			// --- Physical Media
			searcher = new ManagementObjectSearcher(string.Format(Constants.Win32PhysicalMediaQuery, deviceName));
			var serial = string.Empty;
			foreach (var o in searcher.Get())
			{
				if (o is ManagementObject wmiHd)
				{
					serial = wmiHd.GetPropertyValue("SerialNumber").ToString();
				}
				break; // TODO: might not be correct. Was : Exit For
			}

			return serial;

		}
	}
}
