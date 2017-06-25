using System;
using System.Globalization;

namespace Sony.Media.Utilities
{
	/// <summary>
	/// Provides support for storing and working with SMPTE Timecodes
	/// </summary>
	/// <remarks>
	/// LTC includes date / time (HH:MM:SS) and frame (FF)
	/// For convenience, FF is encoded as FF*10 milliseconds.
	/// For example frame 1 is 10 milliseconds, frame 20 is 200 milliseconds.
	/// This is OK for comparison, but no good for direct maths!
	/// Also includes setting for frames per second (fps)
	/// Use 25 for 625 line and 30 for 525 line
	/// </remarks>
	/// <todo>
	/// Provide support for comparisons
	/// Work with changes to daylight savings.
	/// </todo>

	public class LinearTimeCode
	{

		private int _frame;
		private DateTime _dt;
		private int _fps=0; // 25 for 625 line and 30 for 525 line

		// Currently US only formatting - is this a problem?
		private CultureInfo MyCultureInfo = new CultureInfo("en-US");

		public int frame
		/// <param name="frame>frame number for LTC.</param>
		{
			get
			{
				return _frame;
			}
			set
			{
				if (fps == 0)
				{
					throw new ArgumentException("fps isn't set yet. It must be set to 25 or 30","frame");
				}
				if ((value < 0) || (value > fps))
				{
					throw new ArgumentOutOfRangeException("frame is out of range","frame");
				}
				_frame = value;
				_dt = _dt.AddMilliseconds((_frame * 10.0) - _dt.Millisecond);
			}
		}

		public int fps
		/// <param name="fps">frames per second. Use 30 instead of 29.97!</param>
		{
			get
			{
				return _fps;
			}
			set
			{
				if ((value != 25) && (value != 30)) 
				{
					throw new ArgumentOutOfRangeException("fps should be 25 or 30","fps");
				}
				_fps = value;
			}
		}

		public DateTime dt
		/// <param name="dt">Underlying DateTime property. Useful for comparisons.</param>
		{
			get
			{
				return _dt;
			}
			set
			{
				if (fps == 0)
				{
					throw new ArgumentException("fps isn't set yet. It must be set to 25 or 30","datetime");
				}
				// Check for drop frame in initialisation
				if ((fps == 30) && (frame < 2) && (value.Second == 0) && (value.Minute % 10 != 0))
				{
					throw new ArgumentException("Drop frame not allowed","datetime");
				}
				_dt = value;
				_dt = _dt.AddMilliseconds((_frame * 10.0) - _dt.Millisecond);
			}
		}

		public override string ToString()
		{
			if (this.frame < 10) 
			{
				return(this.dt.ToString("s") + ":0" + this.frame.ToString());
			} 
			else 
			{
				return(this.dt.ToString("s") + ":" + this.frame.ToString());
			}
		}

		public double secondsSinceMidnight()
		{
			///<summary>
			///Returns number of seconds in LTC since midnight.
			///Value includes fractional portion for frame number.
			///</summary>
			double secs = (double) dt.Second + dt.Minute * 60 + dt.Hour * 3600;
			return(secs + ((double) frame / (double) fps));
		}

		public void nextFrame()
			///<remarks>
			///Move LTC onto next frame.
			///If in 30fps mode make necessary drop frame adjustment.
			///</remarks>
		{
			int ss = this.dt.Second;
			int mi = this.dt.Minute;
			int hh = this.dt.Hour;
			int dd = this.dt.Day;
			int mm = this.dt.Month;
			int yyyy = this.dt.Year;
			double addSeconds = 0.0;
			// Create return date as date time without frame number in milliseconds.
			// This is done to make sure that errors don't creep in by continually
			// adding 100 * frame
			// It is easier to zero out the milliseconds, and add them later.
			DateTime newDate = new DateTime(yyyy,mm,dd,hh,mi,ss,0);
			frame = (frame + 1) % fps;
			if (frame == 0)
			{
				// Adjust for drop frames.
				// Set frame to 2 every minute (except for multiples of 10)
				// The remainder calculation is for 9 because the minutes haven't
				// yet stepped past 9!
				if ((fps == 30) && (ss == 59) && ((mi % 10) != 9)) 
				{
					frame = 2;
				}
				addSeconds = 1.0;
			}
			addSeconds += (double) frame * 0.01;
			this.dt = newDate.AddSeconds(addSeconds);
		}

		public void addFrames(int numberOfFrames) 
			///<remarks>
			///Move LTC on by a number of frames.
			///With 30fps (29.97) this is difficult.
			///We can rely on the fact that 10 minutes is always 17,982 frames,
			///So, we can first of all move on to the rounded down number of 10 minutes.
			///Just to make life easier, I won't allow values < 0!
			///</remarks>
		{
			if (numberOfFrames < 0) 
			{
				throw new ArgumentOutOfRangeException("numberOfFrames","Can't subtract frames, can only add!");
			}
			int ss = this.dt.Second;
			int mi = this.dt.Minute;
			int hh = this.dt.Hour;
			int dd = this.dt.Day;
			int mm = this.dt.Month;
			int yyyy = this.dt.Year;
			double addSeconds = 0.0;
			// Create return date as date time without frame number in milliseconds.
			// This is done to make sure that errors don't creep in by continually
			// adding (1000 * frame / fps) milliseconds.
			// It is easier to zero out the milliseconds, and add them later.
			DateTime newDate = new DateTime(yyyy,mm,dd,hh,mi,ss,0);
			if (fps == 25) 
			{
				int numberSeconds = numberOfFrames / fps;
				int fractionalPart = numberOfFrames % fps;
				frame = frame + fractionalPart;
				if (frame >= fps)
				{
					frame-=fps;
					numberSeconds++;
				}
				addSeconds = numberSeconds + ((double) (frame) * 0.01);
			}
			else 
			{
				throw new NotSupportedException("Adding frames to 30fps video not yet supported");
				// TODO: Need to try out algorithms.
			}
			this.dt = newDate.AddSeconds(addSeconds);
		}

		public LinearTimeCode(int fps, string dtString, string tcString)
		{
			this.fps = fps;
			string timePart = tcString.Substring(0,8);
			this.frame = int.Parse(tcString.Substring(9,2));
			try
			{
				this.dt = (DateTime.ParseExact(dtString + " " + timePart,"yyyy-MM-dd HH:mm:ss",MyCultureInfo)).AddMilliseconds(0.01 * (double) frame);
			}
			catch (Exception ex)
			{
				throw new ArgumentException("Error in Date or Time String","dtString or tcString",ex);
			}
		}

		public LinearTimeCode(int fps, string dttcString)
		{
			this.fps = fps;
			string timePart = dttcString.Substring(11,8);
			string datePart = dttcString.Substring(0,10);
			this.frame = int.Parse(dttcString.Substring(20,2));
			try
			{
				this.dt = (DateTime.ParseExact(datePart + " " + timePart,"yyyy-MM-dd HH:mm:ss",MyCultureInfo)).AddMilliseconds(0.01 * (double) frame);
			}
			catch (Exception ex)
			{
				throw new ArgumentException("Error in Date or Time String","dttcString",ex);
			}
		}

		public LinearTimeCode() 
		{
		}
	}
}
