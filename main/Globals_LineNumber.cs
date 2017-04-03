using System;
using System.IO;
using System.Drawing;
using System.Diagnostics;

partial class Globals
{
	public static Point GetLineNumberAndPosition(string text, int index)
	{
		int nlLength = Environment.NewLine.Length;
		// get line number
		int lineNumber = 0;
		int lastLineStartIndex = 0;
		for (int i = 0; i < index && i < text.Length - nlLength; ++i)
		{
			string testedSpan = text.Substring(i, nlLength);
			if (String.Compare(testedSpan, Environment.NewLine) == 0)
			{
				lineNumber++;
				lastLineStartIndex = i + nlLength;
			}
		}
		// get position
		int position = 0;
		for (int i = lastLineStartIndex; i < index && i < text.Length; ++i)
		{
			if (text[i] == '\t')
			{
				position += 4;
			}
			else
			{
				position++;
			}
		}
		Point res = new Point(lineNumber+1, position);
		return res;
	}
}
