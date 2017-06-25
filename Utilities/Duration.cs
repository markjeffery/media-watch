using System;

namespace Sony.Media.Utilities
{
	/// <summary>
	/// Support for Durations for assets and chunks.
	/// </summary>
	public class Duration
	{
		private TimeSpan _ts;

		public Duration() 
		{
		}

		public Duration(int Hours, int Minutes, int Seconds, int Frames)
		{
			_ts = new TimeSpan(0,Hours,Minutes,Seconds,Frames * 10);
		}

		public Duration(int fps, int numFrames) 
		{
			// 10,000,000 ticks per second
			// 10,000 ticks per millisecond.
			long nTicks = (numFrames / fps) * 10000000L + ((numFrames % fps) * 10 * 10000);
			_ts = new TimeSpan(nTicks);
		}

		public int getSeconds()
			/// <summary>
			/// Return number of seconds in duration truncated to an integer.
			/// </summary>
		{
			return (int) _ts.TotalSeconds;
		}

		public override string ToString() 
		{
			string retval = _ts.Hours.ToString("00") + ":" + _ts.Minutes.ToString("00") + ":" + _ts.Seconds.ToString("00") + ":" + _ts.Milliseconds.ToString("00");
			return(retval);
		}
	}
}
