namespace com.faith.coreconsole
{
	using UnityEngine;
	using System;
	using System.Text;
	using System.Collections.Generic;

    public class CoreConsoleUtility
    {
		#region Configuretion


		private static string DecimalToHexNumeric(int value)
		{

			if (value == -1)
				return ".";
			else if (value == 10)
				return "A";
			else if (value == 11)
				return "B";
			else if (value == 12)
				return "C";
			else if (value == 13)
				return "D";
			else if (value == 14)
				return "E";
			else if (value == 15)
				return "F";
			else
				return value.ToString();
		}

		#endregion

		#region Public Callback

		public static string TruncateAllWhiteSpace(string t_ModifiableString)
		{

			string[] t_SplitByWhiteSpace = t_ModifiableString.Split(' ');
			string t_NewString = "";
			foreach (string t_SubString in t_SplitByWhiteSpace)
			{

				t_NewString += t_SubString;
			}

			return t_NewString;
		}

		public static bool IsSameString(string t_String1, string t_String2, bool t_TruncateWhiteSpace = false)
		{
			if (t_TruncateWhiteSpace)
			{

				t_String1 = TruncateAllWhiteSpace(t_String1);
				t_String2 = TruncateAllWhiteSpace(t_String2);
			}
			return String.Equals(t_String1, t_String2);
		}

		public static bool IsSameStringCaseInsensitive(string t_String1, string t_String2, bool t_TruncateWhiteSpace = false)
		{

			if (t_TruncateWhiteSpace)
			{
				t_String1 = TruncateAllWhiteSpace(t_String1);
				t_String2 = TruncateAllWhiteSpace(t_String2);
			}
			return String.Equals(t_String1, t_String2, StringComparison.OrdinalIgnoreCase);
		}

		//Method Source	:	UnityTechnology Editor SourceCode
		public static string StacktraceWithHyperlinks(string stacktraceText)
		{
			StringBuilder textWithHyperlinks = new StringBuilder();
			var lines = stacktraceText.Split(new string[] { "\n" }, StringSplitOptions.None);
			for (int i = 0; i < lines.Length; ++i)
			{
				string textBeforeFilePath = ") (at ";
				int filePathIndex = lines[i].IndexOf(textBeforeFilePath, StringComparison.Ordinal);
				if (filePathIndex > 0)
				{
					filePathIndex += textBeforeFilePath.Length;
					if (lines[i][filePathIndex] != '<') // sometimes no url is given, just an id between <>, we can't do an hyperlink
					{
						string filePathPart = lines[i].Substring(filePathIndex);
						int lineIndex = filePathPart.LastIndexOf(":", StringComparison.Ordinal); // LastIndex because the url can contain ':' ex:"C:"
						if (lineIndex > 0)
						{
							int endLineIndex = filePathPart.LastIndexOf(")", StringComparison.Ordinal); // LastIndex because files or folder in the url can contain ')'
							if (endLineIndex > 0)
							{
								string lineString =
									filePathPart.Substring(lineIndex + 1, (endLineIndex) - (lineIndex + 1));
								string filePath = filePathPart.Substring(0, lineIndex);

								textWithHyperlinks.Append(lines[i].Substring(0, filePathIndex));
								textWithHyperlinks.Append("<a href=\"" + filePath + "\"" + " line=\"" + lineString + "\">");
								textWithHyperlinks.Append(filePath + ":" + lineString);
								textWithHyperlinks.Append("</a>)\n");

								continue; // continue to evade the default case
							}
						}
					}
				}
				// default case if no hyperlink : we just write the line
				textWithHyperlinks.Append(lines[i] + "\n");
			}
			// Remove the last \n
			if (textWithHyperlinks.Length > 0) // textWithHyperlinks always ends with \n if it is not empty
				textWithHyperlinks.Remove(textWithHyperlinks.Length - 1, 1);

			return textWithHyperlinks.ToString();
		}

		public static string GetHexValue(float value, bool considerDecimalPoint = false)
		{

			if (value == 0)
				return "0";

			Stack<float> stackForDecimal = new Stack<float>();

			while (value > 0)
			{

				if (value < 16)
				{

					float floatValue = value % 16;
					stackForDecimal.Push((int)floatValue);

					if (!considerDecimalPoint)
					{
						value = 0;
						break;
					}
					else
					{

						//Decimal Fraction
						//----------------
						stackForDecimal.Push(-1);
						float decimalFraction = floatValue - ((int)floatValue);
						if (decimalFraction > 0)
							stackForDecimal.Push((int)(decimalFraction * 16));
						//----------------

						value = 0;
					}
				}
				else
				{

					stackForDecimal.Push(value % 16);
					value /= 16;
				}
			}

			string result = "";
			while (stackForDecimal.Count > 0)
			{
				int integerValue = (int)stackForDecimal.Pop();
				string hexValue = DecimalToHexNumeric(integerValue);
				result += hexValue;
			}

			string[] splitByDecimalPoint = result.Split('.');
			if (splitByDecimalPoint.Length > 1)
				return splitByDecimalPoint[1] + "." + splitByDecimalPoint[0];
			else
				return splitByDecimalPoint[0];
		}

		public static string GetHexColorFromRGBColor(Color color)
		{

			Vector3 _32BitColor = new Vector3(
				color.r * 255,
				color.g * 255,
				color.b * 255);

			return "#"
				+ (_32BitColor.x < 16 ? "0" : "") + GetHexValue(_32BitColor.x)
				+ (_32BitColor.y < 16 ? "0" : "") + GetHexValue(_32BitColor.y)
				+ (_32BitColor.z < 16 ? "0" : "") + GetHexValue(_32BitColor.z);
		}


		#endregion


	}
}

