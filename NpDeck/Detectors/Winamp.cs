using System.Text.RegularExpressions;

namespace NpDeck.Detectors
{
    public class Winamp: IDetector
    {
		private static readonly Regex Extractor = new Regex("^(.+)\\s*-\\s*Winamp", RegexOptions.IgnoreCase);
        public Result Detect()
        {
	        return Util.ResultFromWindowClassAndRegex("Winamp v1.x", Extractor);
        }
    }
}