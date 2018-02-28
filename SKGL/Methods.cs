//Copyright (C) 2011-2012 Artem Los, www.clizware.net.
//The author of this code shall get the credits

// This project uses two general algorithms:
//  - Artem's Information Storage Format (Artem's ISF-2)
//  - Artem's Serial Key Algorithm (Artem's SKA-2)

// A great thank to Iberna (https://www.codeplex.com/site/users/view/lberna)
// for getHardDiskSerial algorithm.

using System;
using System.Numerics;
#if DEBUG
using System.Runtime.CompilerServices;
#endif

#if DEBUG
[assembly: InternalsVisibleTo("SKGLTest")]
#endif
namespace SKGL
{
	#region "T H E  C O R E  O F  S K G L"
	internal class Methods : SerialKeyConfiguration
	{

		//The construction of the key
		protected internal string _encrypt(int days, bool[] tfg, string secretPhrase, int id, DateTime creationDate)
		{
			// This function will store information in Artem's ISF-2
			//Random variable was moved because of the same key generation at the same time.

			var retInt = Convert.ToInt32(creationDate.ToString("yyyyMMdd"));
			// today

			decimal result = 0;

			result += retInt;
			// adding the current date; the generation date; today.
			result *= 1000;
			// shifting three times at left

			result += days;
			// adding time left
			result *= 1000;
			// shifting three times at left

			result += BooleanToInt(tfg);
			// adding features
			result *= 100000;
			//shifting three times at left

			result += id;
			// adding random ID

			// This part of the function uses Artem's SKA-2

			if (string.IsNullOrEmpty(secretPhrase) | secretPhrase == null)
			{
				// if not password is set, return an unencrypted key
				return Base10ToBase26((GetEightByteHash(result.ToString()) + result.ToString()));
			}
			else
			{
				// if password is set, return an encrypted 
				return Base10ToBase26((GetEightByteHash(result.ToString()) + _encText(result.ToString(), secretPhrase)));
			}


		}
		protected internal string _decrypt(string key, string secretPhrase)
		{
			if (string.IsNullOrEmpty(secretPhrase) | secretPhrase == null)
			{
				// if not password is set, return an unencrypted key
				return Base26ToBase10(key);
			}
			else
			{
				// if password is set, return an encrypted 
				var usefulInformation = Base26ToBase10(key);
				return usefulInformation.Substring(0, 9) + _decText(usefulInformation.Substring(9), secretPhrase);
			}

		}
		//Deeper - encoding, decoding, et cetera.

		//Convertions, et cetera.----------------
		protected internal int BooleanToInt(bool[] booleanArray)
		{
			var aVector = 0;
			//
			//In this function we are converting a binary value array to a int
			//A binary array can max contain 4 values.
			//Ex: new boolean(){1,1,1,1}

			for (var i = 0; i < booleanArray.Length; i++)
			{
				switch (booleanArray[i])
				{
					case true:
						aVector += Convert.ToInt32((Math.Pow(2, (booleanArray.Length - i - 1))));
						// times 1 has been removed
						break;
				}
			}
			return aVector;
		}
		protected internal bool[] IntToBoolean(int num)
		{
			//In this function we are converting an integer (created with privious function) to a binary array

			var bReturn = Convert.ToInt32(Convert.ToString(num, 2));
			var aReturn = Return_Length(bReturn.ToString(), 8);
			var cReturn = new bool[8];


			for (var i = 0; i <= 7; i++)
			{
				cReturn[i] = aReturn.Substring(i, 1) == "1";
			}
			return cReturn;
		}
		protected internal string _encText(string inputPhase, string secretPhrase)
		{
			//in this class we are encrypting the integer array.
			var res = "";

			for (var i = 0; i <= inputPhase.Length - 1; i++)
			{
				res += Modulo(Convert.ToInt32(inputPhase.Substring(i, 1)) + Convert.ToInt32(secretPhrase.Substring(Modulo(i, secretPhrase.Length), 1)), 10);
			}

			return res;
		}
		protected internal string _decText(string encryptedPhase, string secretPhrase)
		{
			//in this class we are decrypting the text encrypted with the function above.
			var res = "";

			for (var i = 0; i <= encryptedPhase.Length - 1; i++)
			{
				res += Modulo(Convert.ToInt32(encryptedPhase.Substring(i, 1)) - Convert.ToInt32(secretPhrase.Substring(Modulo(i, secretPhrase.Length), 1)), 10);
			}

			return res;
		}
		protected internal string Return_Length(string number, int length)
		{
			// This function create 3 length char ex: 39 to 039
			if ((number.Length != length))
			{
				while (number.Length != length)
				{
					number = "0" + number;
				}
			}
			return number;
			//Return Number

		}
		protected internal int Modulo(int num, int _base)
		{
			// canged return type to integer.
			//this function simply calculates the "right modulo".
			//by using this function, there won't, hopefully be a negative
			//number in the result!
			return num - _base * Convert.ToInt32(Math.Floor((decimal)num / (decimal)_base));
		}
		protected internal string TwentyfiveByteHash(string s)
		{
			var amountOfBlocks = s.Length / 5;
			var preHash = new string[amountOfBlocks + 1];

			if (s.Length <= 5)
			{
				//if the input string is shorter than 5, no need of blocks! 
				preHash[0] = GetEightByteHash(s).ToString();
			}
			else if (s.Length > 5)
			{
				//if the input is more than 5, there is a need of dividing it into blocks.
				for (var i = 0; i <= amountOfBlocks - 2; i++)
				{
					preHash[i] = GetEightByteHash(s.Substring(i * 5, 5)).ToString();
				}

				preHash[preHash.Length - 2] = GetEightByteHash(s.Substring((preHash.Length - 2) * 5, s.Length - (preHash.Length - 2) * 5)).ToString();
			}
			return string.Join("", preHash);
		}
		protected internal int GetEightByteHash(string s, int mustBeLessThan = 1000000000)
		{
			//This function generates a eight byte hash

			//The length of the result might be changed to any length
			//just set the amount of zeroes in MUST_BE_LESS_THAN
			//to any length you want
			uint hash = 0;

			foreach (var b in System.Text.Encoding.Unicode.GetBytes(s))
			{
				hash += b;
				hash += (hash << 10);
				hash ^= (hash >> 6);
			}

			hash += (hash << 3);
			hash ^= (hash >> 11);
			hash += (hash << 15);

			var result = (int)(hash % mustBeLessThan);

			//we want the result to not be zero, as this would thrown an exception in check.
			if (result == 0)
				result = 1;


			var check = mustBeLessThan / result;

			if (check > 1)
			{
				result *= check;
			}

			//when result is less than MUST_BE_LESS_THAN, multiplication of result with check will be in that boundary.
			//otherwise, we have to divide by 10.
			if (mustBeLessThan == result)
				result /= 10;


			return result;
		}

		/// <summary>
		/// Converts a base 10 number to base 26 number.
		/// Remember that s is a decimal, and the size is limited. 
		/// In order to get size, type Decimal.MaxValue.
		/// </summary>
		/// <remarks>
		/// Note that this method will still work, even though you only 
		/// can add, subtract numbers in range of 15 digits.
		/// </remarks>
		/// <param name="s">The decimal string to convert.</param>
		/// <returns>System.String.</returns>
		protected internal string Base10ToBase26(string s)
		{
			var allowedLetters = Constants.AllowedLetters.ToCharArray();

			var num = Convert.ToDecimal(s);

			var result = new char[s.Length + 1];
			var j = 0;
			while ((num >= 26))
			{
				var reminder = Convert.ToInt32(num % 26);
				result[j] = allowedLetters[reminder];
				num = (num - reminder) / 26;
				j += 1;
			}

			result[j] = allowedLetters[Convert.ToInt32(num)];
			// final calculation

			var returnNum = "";
			for (var k = j; k >= 0; k -= 1)  // not sure
			{
				returnNum += result[k];
			}
			return returnNum;

		}
		/// <summary>
		/// Base26s to base10.
		/// This function will convert a number that has been generated
		/// with functin above, and get the actual number in decimal
		/// This function requires Mega Math to work correctly.
		/// </summary>
		/// <param name="s">The string to convert.</param>
		/// <returns>System.String.</returns>
		protected internal string Base26ToBase10(string s)
		{
			var result = new BigInteger();
			for (var i = 0; i <= s.Length - 1; i += 1)
			{
				var pow = Powof(26, (s.Length - i - 1));
				result = result + Constants.AllowedLetters.IndexOf(s.Substring(i, 1), StringComparison.Ordinal) * pow;
			}
			return result.ToString(); //not sure
		}

		/// <summary>
		/// Powofs the specified x.
		/// Because of the uncertain answer using Math.Pow and ^, 
		/// this function is here to solve that issue.
		/// It is currently using the MegaMath library to calculate.
		/// </summary>
		/// <param name="x">The x.</param>
		/// <param name="y">The y.</param>
		/// <returns>BigInteger.</returns>
		protected internal BigInteger Powof(int x, int y)
		{
			BigInteger newNum = 1;
			switch (y)
			{
				case 0:
					// if 0, return 1, e.g. x^0 = 1 (mathematicaly proven!) 
					return 1;
				case 1:
					// if 1, return x, which is the base, e.g. x^1 = x
					return x;
				default:
					// if both conditions are not satisfied, this loop
					// will continue to y, which is the exponent.
					for (var i = 0; i <= y - 1; i++)
					{
						newNum = newNum * x;
					}
					return newNum;
			}
		}
	}

	#endregion
}
