using System.Text.RegularExpressions;

namespace NpDeck.Detectors
{
	class Foobar2000: IDetector
	{
		private static readonly Regex Extractor = new Regex("^(.+)\\s*\\[foobar2000", RegexOptions.IgnoreCase);
		public Result Detect()
		{
			return Util.ResultFromWindowClassAndRegex("{97E27FAA-C0B3-4b8e-A693-ED7881E99FC1}", Extractor);
		}
	}
}
