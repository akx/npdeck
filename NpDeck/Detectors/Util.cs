using System;
using System.Text.RegularExpressions;

namespace NpDeck.Detectors
{
	static class Util
	{
		public static Result ResultFromWindowClassAndRegex(string className, Regex regex=null)
		{
			var text = WinApi.GetWindowTextByClassName(className);
			if (!String.IsNullOrWhiteSpace(text))
			{
				if (regex != null)
				{
					var m = regex.Match(text);
					if (m.Success && m.Groups.Count > 1) text = m.Groups[1].Value;
				}
				return new Result(text);
			}
			return null;
		}
	}
}
