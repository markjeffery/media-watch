using System;
using System.Threading;
using System.Data;
using System.Globalization;
using Sony.Media.Utilities;

namespace Populate
{
	/// <summary>
	/// Populate the asrun table.
	/// Produce lots of xml files in the hotasrun folder
	/// to be picked up by the HotFolderMonitor
	/// </summary>
	public class AsRunDataset
	{
		private int _numDays = 0;
		private int _numChannels = 0;
		private int[] _progList;
		private int iChannel = 1;
		private int iSeconds = 0;
		private int iRecIndex = 0;
		private int iTitleIndex = 0;
		private LinearTimeCode iltc = new LinearTimeCode();
		private Duration idur = new Duration();

		public AsRunDataset()
		{
		}
		public int NumDays 
		{
			get 
			{
				return _numDays;
			}
			set 
			{
				if (value <= 0) 
				{
					throw new ArgumentOutOfRangeException("Number of Days must be >0");
				} 
				_numDays = value;
			}
		}

		public int NumChannels
		{
			get
			{
				return _numChannels;
			}
			set 
			{
				if (value <= 0) 
				{
					throw new ArgumentOutOfRangeException("Number of Channels must be >0");
				} 
				_numChannels = value;
			}
		}

		public int[] ProgList
		{
			get
			{
				return _progList;
			}
			set
			{
				if (value.Length == 0)
				{
					throw new ArgumentNullException("ProgList","No programmes in programme list");
				}
				_progList = new int[value.Length];
				for (int i=0; i < value.Length; i++) 
				{
					_progList[i] = value[i];
				}
			}
		}

		public void StartPopulate() 
		{
			if (_numDays <= 0) 
			{
				throw new ArgumentOutOfRangeException("Number of Days must be >0");
			}
			if (_numChannels <= 0)
			{
				throw new ArgumentOutOfRangeException("Number of Channels must be >0");
			}
			if (_progList.Length == 0)
			{
				throw new ArgumentNullException("ProgList","No programmes in programme list");
			}
			iChannel = 1;
			iltc = new LinearTimeCode(25,"2000-01-01 00:00:00:00");
			iSeconds = 0;
			iTitleIndex = 0;
			iRecIndex++;
		}

		public int Populate() 
		{
			// Populate part of the database, returning after a while
			// to update the progress meter.
			DataSet ds = new DataSet("asrun");
			ds.ReadXmlSchema("C:\\Program Files\\sony\\MediaWatchDatabase\\populate\\AsRunSchema.xsd");
			int numRecords = 0;
			string[] insTitles = {"Fimbles","Pingu","Captain Abercromby","Chuckle Vision","Tom and Jerry Kids",
									 "Taz-Mania","UBOS","Arthur","Rugrats","The Saturday Show","12noon BBC News",
									 "Grandstand","Athletics","Focus","British Superbikes"};

			long numSeconds = _numDays * 86400;
			for (int i=0; i < 10; i++) // Do 10 chunks before returning.
			{
				if (iSeconds < numSeconds) 
				{
					for (int progIndex=0; ((progIndex < ProgList.Length) && (iSeconds < numSeconds)); progIndex++) 
					{
						Duration idur = new Duration(25, ProgList[progIndex] * 25);
						iTitleIndex = ((iTitleIndex+1) % insTitles.Length);
					    DataRow dr = ds.Tables["asrun"].NewRow();
						
						dr["startDate"] = iltc.dt.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo);
						dr["startTimeCode"] = iltc.dt.ToString("HH:mm:ss:ff", DateTimeFormatInfo.InvariantInfo);
						dr["duration"] = idur.ToString();
						dr["channel"] = "CH" + iChannel.ToString();
						dr["title"] = insTitles[iTitleIndex];
						dr["id"] = "id column";
						dr["s"] = "s column";
						dr["status"] = "status column";
						dr["ch"] = "ch column";
						dr["reconcile"] = "reconcile column";
						dr["ty"] = "ty column";
						ds.Tables["asrun"].Rows.Add(dr);

						string fname = "C:\\Program Files\\sony\\MediaWatchServer\\asrunhotfolder\\PROG-{0}.xml";
						fname = String.Format(fname, iRecIndex.ToString());
						iRecIndex++;
						ds.WriteXml(fname,XmlWriteMode.IgnoreSchema);
						ds.Clear();
						numRecords++;

						// Now calculate next date and time
						iSeconds += idur.getSeconds();
						iltc.dt = iltc.dt.AddSeconds((double) (idur.getSeconds()));

					}
				}
				else
				{
					iSeconds = 0;
					iltc = new LinearTimeCode(25,"2000-01-01 00:00:00:00");
					if (iChannel++ >= _numChannels)
					{
						return(-1);
					}
				}
			}
			return(numRecords);
		}

	}
}
