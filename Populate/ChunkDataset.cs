using System;
using System.Threading;
using System.Data;
using System.Globalization;
using Sony.Media.Utilities;

namespace Populate
{
	/// <summary>
	/// Dataset used to populate chunk data
	/// </summary>
	public class ChunkDataset
	{
		private int _numDays = 0;
		private int _numChannels = 0;
		private int _chunkSize = 0;
		private int iChannel = 1;
		private int iSeconds = 0;
		private int iRecIndex = 0;
		private LinearTimeCode iltc = new LinearTimeCode();
		private Duration idur = new Duration();

		public ChunkDataset()
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

		public int ChunkSize
		{
			get
			{
				return _chunkSize;
			}
			set
			{
				if (value <= 0) 
				{
					throw new ArgumentOutOfRangeException("Chunk Size must be >0");
				} 
				_chunkSize = value;
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
			if (_chunkSize <= 0)
			{
				throw new ArgumentOutOfRangeException("Chunk Size must be >0");
			}
			iChannel = 1;
			iltc = new LinearTimeCode(25,"2000-01-01 00:00:00:00");
			iSeconds = 0;
			idur = new Duration(25,ChunkSize * 60 * 25);
			iRecIndex++;
		}

		public int Populate() 
		{
			// Populate part of the database, returning after a while
			// to update the progress meter.
			DataSet ds = new DataSet("chunk");
			ds.ReadXmlSchema("C:\\Program Files\\sony\\MediaWatchDatabase\\populate\\ChunkSchema.xsd");
			int numRecords = 0;
			string[] insurls = {"asseta","assetb","assetc","assetd","assete","assetf","assetg","asseth","asseti",
			"assetj","assetk","assetl","assetm","assetn","asseto","assetp","assetq","assetr",
			"assets","assett","assetu","assetv","assetw","assetx","assety","assetz"};
			int urlIndex = 0;

			long numSeconds = _numDays * 86400;
			for (int i=0; i < 10; i++) // Do 10 chunks before returning.
			{
				if (iSeconds < numSeconds) 
				{
					urlIndex = ((urlIndex+1) % insurls.Length);
					DataRow dr = ds.Tables["asset"].NewRow();
				 
					dr["fileName"] = insurls[urlIndex];
					dr["fileDuration"] = idur.ToString();
					dr["startDate"] = iltc.dt.ToString("yyyy-MM-dd", DateTimeFormatInfo.InvariantInfo);
					dr["startYear"] = iltc.dt.ToString("yyyy", DateTimeFormatInfo.InvariantInfo);
					dr["startMonth"] = iltc.dt.ToString("MM", DateTimeFormatInfo.InvariantInfo);
					dr["startDay"] = iltc.dt.ToString("dd", DateTimeFormatInfo.InvariantInfo);
					dr["startMonthAsText"] = iltc.dt.ToString("MMM", DateTimeFormatInfo.InvariantInfo);
					dr["startTimeCode"] = iltc.dt.ToString("HH:mm:ss:ff", DateTimeFormatInfo.InvariantInfo);
					dr["startTime"] = iltc.dt.ToString("HH:mm:ss", DateTimeFormatInfo.InvariantInfo);
					dr["sessionID"] = "14";
					dr["recorderName"] = "gbbprlbasws141";
					dr["inputID"] = iChannel.ToString();
					dr["userID"] = "CH" + iChannel.ToString();

					ds.Tables["asset"].Rows.Add(dr);

					string fname = "C:\\Program Files\\sony\\MediaWatchServer\\OutputHotFolder\\PROG-{0}.xml";
					fname = String.Format(fname, iRecIndex.ToString());
					iRecIndex++;
					ds.WriteXml(fname,XmlWriteMode.IgnoreSchema);
					ds.Clear();

					numRecords++;
					// Now calculate next date and time
					iSeconds += _chunkSize * 60;
					iltc.dt = iltc.dt.AddSeconds((double) (_chunkSize * 60));
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
