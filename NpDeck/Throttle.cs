using System;

namespace NpDeck
{
	public class Throttle
	{
		public Action _action;
		private readonly TimeSpan _minInterval;
		private DateTime _lastCall;

		public Throttle(Action action, TimeSpan minInterval)
		{
			_action = action;
			_minInterval = minInterval;
			_lastCall = DateTime.Now;
		}

		public void Call()
		{
			var now = DateTime.Now;
			if (now - _lastCall < _minInterval) return;
			_lastCall = now;
			_action();
		}
	}
}