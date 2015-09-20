using System;

namespace NpDeck.Detectors
{
	public class Result
	{
		public Result(string title)
		{
			if (String.IsNullOrWhiteSpace(title)) title = String.Empty;
			Title = title.Trim();
		}

		public string Title { get; private set; }

	}
}