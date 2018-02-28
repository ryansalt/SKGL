namespace SKGL
{
    internal static class Constants
	{
		internal const string Win32ProcessorQuery = "select * from Win32_Processor";
		internal const string Win32BiosQuery = "select * from Win32_BIOS";
		internal const string Win32BaseboardQuery = "select * from Win32_BaseBoard";
		internal const string EmptyBiosSerialNumber = "To be filled by O.E.M.";
		internal const string Win32BootPartitionQuery = "select * from Win32_DiskPartition WHERE BootPartition=True";
		internal const string Win32DriveQuery = "SELECT * FROM Win32_DiskDrive where Index = {0}";
		internal const string Win32PhysicalMediaQuery = "SELECT * FROM Win32_PhysicalMedia WHERE Tag like '{0}'";
		internal const string AllowedLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	}
}
