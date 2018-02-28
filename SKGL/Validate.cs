//Copyright (C) 2011-2012 Artem Los, www.clizware.net.
//The author of this code shall get the credits

// This project uses two general algorithms:
//  - Artem's Information Storage Format (Artem's ISF-2)
//  - Artem's Serial Key Algorithm (Artem's SKA-2)

// A great thank to Iberna (https://www.codeplex.com/site/users/view/lberna)
// for getHardDiskSerial algorithm.

using System;

namespace SKGL
{
	#region "DECRYPTION"
	public class Validate : BaseConfiguration
	{
		//this class have to be inherited becuase of the key which is shared with both encryption/decryption classes.

		//TODO: If this is meant to be used, fix code, otherwise lose it
		private SerialKeyConfiguration _skc = new SerialKeyConfiguration();
		private readonly Methods _a = new Methods();
		private string _res = "";

		public Validate()
		{
			// No overloads works with Sub New
		}
		public Validate(SerialKeyConfiguration serialKeyConfiguration)
		{
			_skc = serialKeyConfiguration;
		}

		private string _secretPhrase = "";
		/// <summary>
		/// If the key has been encrypted, when it was generated, please set the same SecretPhrase here.
		/// </summary>
		public string SecretPhrase
		{
			get => _secretPhrase;
			set
			{
				if (value != _secretPhrase)
				{
					_secretPhrase = _a.TwentyfiveByteHash(value);
				}
			}
		}
	
		private void DecodeKeyToString()
		{
			// checking if the key already have been decoded.
			if (string.IsNullOrEmpty(_res) | _res == null)
			{

				var stageOne = "";

				Key = Key.Replace("-", "");
				//if the admBlock has been changed, the getMixChars will be executed.
				stageOne = Key;

				if (!string.IsNullOrEmpty(SecretPhrase) | SecretPhrase != null)
				{
					//if no value "SecretPhrase" given, the code will directly decrypt without using somekind of encryption
					//if some kind of value is assigned to the variable "SecretPhrase", the code will execute it FIRST.
					//the SecretPhrase shall only consist of digits!
					var reg = new System.Text.RegularExpressions.Regex("^\\d$");
					//cheking the string
					if (reg.IsMatch(SecretPhrase))
					{
						//throwing new exception if the string contains non-numrical letters.
						throw new ArgumentException("The SecretPhrase consist of non-numerical letters.");
					}
				}
				_res = _a._decrypt(stageOne, SecretPhrase);
			}
		}

		/// <summary>
		/// Checks whether the key has been modified or not. If the key has been modified - returns false; if the key has not been modified - returns true.
		/// </summary>
		public bool IsValid
		{
			get
			{
				//Dim _a As New methods ' is only here to provide the geteighthashcode method
				try
				{
					if (Key.Contains("-"))
					{
						if (Key.Length != 23)
						{
							return false;
						}
					}
					else
					{
						if (Key.Length != 20)
						{
							return false;
						}
					}
					DecodeKeyToString();

					var decodedHash = _res.Substring(0, 9);
					var calculatedHash = _a.GetEightByteHash(_res.Substring(9, 19)).ToString().Substring(0, 9);
					// changed Math.Abs(_res.Substring(0, 17).GetHashCode).ToString.Substring(0, 8)

					//When the hashcode is calculated, it cannot be taken for sure, 
					//that the same hash value will be generated.
					//learn more about this issue: http://msdn.microsoft.com/en-us/library/system.object.gethashcode.aspx
					return decodedHash == calculatedHash;
				}
				catch
				{
					//if something goes wrong, for example, when decrypting, 
					//this function will return false, so that user knows that it is unvalid.
					//if the key is valid, there won't be any errors.
					return false;
				}
			}
		}
		
		/// <summary>
		/// If the key has expired - returns true; if the key has not expired - returns false.
		/// </summary>
		public bool IsExpired => DaysLeft <= 0;

		public DateTime CreationDate
		{
			get
			{
				DecodeKeyToString();
				return new DateTime(Convert.ToInt32(_res.Substring(9, 4)), Convert.ToInt32(_res.Substring(13, 2)), Convert.ToInt32(_res.Substring(15, 2)));
			}
		}

		public int DaysLeft
		{
			get
			{
				DecodeKeyToString();
				return Convert.ToInt32((ExpireDate - DateTime.Today).TotalDays); //or viseversa
			}
		}

		/// <summary>
		/// Returns the actual amount of days that were set when the key was generated.
		/// </summary>
		public int SetTime
		{
			get
			{
				DecodeKeyToString();
				return Convert.ToInt32(_res.Substring(17, 3));
			}
		}

		/// <summary>
		/// Returns the date when the key is to be expired.
		/// </summary>
		public DateTime ExpireDate
		{
			get
			{
				DecodeKeyToString();
				return CreationDate.AddDays(SetTime);
			}
		}

		/// <summary>
		/// Returns all 8 features in a boolean array
		/// </summary>
		public bool[] Features
		{
			get
			{
				//we already have defined Features in the BaseConfiguration class. 
				//Here we only change it to Read Only.
				DecodeKeyToString();
				return _a.IntToBoolean(Convert.ToInt32(_res.Substring(20, 3)));
			}
		}

		/// <summary>
		/// If the current machine's machine code is equal to the one that this key is designed for, return true.
		/// </summary>
		/// <value></value>
		/// <returns></returns>
		/// <remarks></remarks>
		public bool IsOnRightMachine
		{
			get
			{
				var decodedMachineCode = Convert.ToInt32(_res.Substring(23, 5));

				return decodedMachineCode == MachineCode;
			}
		}
	}
	#endregion
}
