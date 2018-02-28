//Copyright (C) 2011-2012 Artem Los, www.clizware.net.
//The author of this code shall get the credits

// This project uses two general algorithms:
//  - Artem's Information Storage Format (Artem's ISF-2)
//  - Artem's Serial Key Algorithm (Artem's SKA-2)

// A great thank to Iberna (https://www.codeplex.com/site/users/view/lberna)
// for getHardDiskSerial algorithm.

namespace SKGL
{
	public abstract class BaseConfiguration
	{
		/// <summary>
		/// The key will be stored here
		/// </summary>
		public virtual string Key { get; set; } = "";

		/// <summary>
		/// 
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public virtual int MachineCode => MachineCodeManager.GetMachineCode();


	}
}
