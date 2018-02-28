//Copyright (C) 2011-2012 Artem Los, www.clizware.net.
//The author of this code shall get the credits

// This project uses two general algorithms:
//  - Artem's Information Storage Format (Artem's ISF-2)
//  - Artem's Serial Key Algorithm (Artem's SKA-2)

// A great thank to Iberna (https://www.codeplex.com/site/users/view/lberna)
// for getHardDiskSerial algorithm.

namespace SKGL
{
	public class SerialKeyConfiguration : BaseConfiguration
	{
		public virtual bool[] Features
		{
			//will be changed in validating class.
			get;
			set;
		} = {
			false,
			false,
			false,
			false,
			false,
			false,
			false,
			false
			//the default value of the Fetures array.
		};

		public bool AddSplitChar { get; set; } = true;
	}
}
