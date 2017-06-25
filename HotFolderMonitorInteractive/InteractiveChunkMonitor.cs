using System;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Collections;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;
using Sony.Media.Utilities;

namespace HotFolderMonitorInteractive
{
	/// <summary>
	/// Summary description for InteractiveChunkMonitor.
	/// </summary>
	public class InteractiveChunkMonitor : System.Windows.Forms.Form
	{
		private System.Data.SqlClient.SqlConnection mwConnection;

		private string logFile;
		private string urlPrefix;

		private int numberOfChunkFiles = 0;

		private int numberOfAsrunFiles = 0;

		/* Data for channels */
		private DataSet dsChannels;
		private System.Windows.Forms.ListBox messageListBox;

		private Config cnf;

		// List of folders to be watched.
		private Hashtable chunkList = new Hashtable();
		private Hashtable asrunList = new Hashtable();

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public InteractiveChunkMonitor()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// Get pref for log directory and System Config file.
			//
			try 
			{
				System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
				logFile = ((string)(configurationAppSettings.GetValue("logFile", typeof(string))));
				urlPrefix = ((string)(configurationAppSettings.GetValue("urlPrefix", typeof(string))));

				cnf = new Config();
				cnf.startWatching();

				//
				// Get Channel information into new dataset.%
				//
				dsChannels = new DataSet("channels");
				String sql = "select * from channel";
				mwConnection.Open();
				SqlDataAdapter daChannel = new SqlDataAdapter(sql, mwConnection);
				daChannel.Fill(dsChannels, "channels");
				mwConnection.Close();

				// Start FileSystemWatcher for each unique hotfolder for chunks and asrun
				foreach (DataRow row in dsChannels.Tables["channels"].Rows)
				{
					if (!chunkList.ContainsKey(row["hotfolder_chunk"].ToString()))
					{
						FileSystemWatcher chunkWatcher = new FileSystemWatcher(row["hotfolder_chunk"].ToString(),"*.xml");
						chunkWatcher.Created += new FileSystemEventHandler(NewChunkEvent);
						chunkWatcher.EnableRaisingEvents = true;
						chunkList.Add(row["hotfolder_chunk"].ToString(),chunkWatcher);
					}
					if (!asrunList.ContainsKey(row["hotfolder_asrun"].ToString()))
					{
						FileSystemWatcher asrunWatcher = new FileSystemWatcher(row["hotfolder_asrun"].ToString(),"*.xml");
						asrunWatcher.Created += new FileSystemEventHandler(NewAsrunEvent);
						asrunWatcher.EnableRaisingEvents = true;
						asrunList.Add(row["hotfolder_asrun"].ToString(),asrunWatcher);
					}
				}
				mwConnection.Close();
			}
			catch (Exception ex) 
			{
				Console.WriteLine("Unknown exception" + ex.ToString());
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		static void Main() 
		{
			Application.Run(new InteractiveChunkMonitor());		
		}


		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
			this.mwConnection = new System.Data.SqlClient.SqlConnection();
			this.messageListBox = new System.Windows.Forms.ListBox();
			this.SuspendLayout();
			// 
			// mwConnection
			// 
			this.mwConnection.ConnectionString = ((string)(configurationAppSettings.GetValue("mwConnection.ConnectionString", typeof(string))));
			// 
			// messageListBox
			// 
			this.messageListBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.messageListBox.ItemHeight = 16;
			this.messageListBox.Location = new System.Drawing.Point(0, 0);
			this.messageListBox.Name = "messageListBox";
			this.messageListBox.ScrollAlwaysVisible = true;
			this.messageListBox.Size = new System.Drawing.Size(979, 692);
			this.messageListBox.TabIndex = 0;
			// 
			// InteractiveChunkMonitor
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.ClientSize = new System.Drawing.Size(979, 698);
			this.Controls.Add(this.messageListBox);
			this.Name = "InteractiveChunkMonitor";
			this.Text = "InteractiveChunkMonitor";
			this.ResumeLayout(false);

		}
		#endregion

		private void NewChunkEvent(object sender, System.IO.FileSystemEventArgs e)
		{
			int numFilesInThisPass = 0;
			do 
			{
				numFilesInThisPass = 0;
				DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(e.FullPath));
				// Create an array representing the files in the current directory.
				FileInfo[] fi = di.GetFiles();
				foreach (FileInfo fiTemp in fi) 
				{
					if (fiTemp.Extension.Equals(".xml")) 
					{
						string thisFilePath = fiTemp.FullName;
						DataSet dsChunk = new DataSet("assets");
						try
						{
							dsChunk.ReadXml(thisFilePath,XmlReadMode.InferSchema);
							File.Delete(thisFilePath);
							archiveDataset(dsChunk);
							numberOfChunkFiles++;
							numFilesInThisPass++;
							// Look up channel name against dsChannels dataset.
							DataView dvChannel = new DataView(dsChannels.Tables["channels"], "", "name", DataViewRowState.CurrentRows);
							DataTable dtChunk = dsChunk.Tables["Asset"];
							foreach (DataRow drChunk in dtChunk.Rows)
							{
								int rowIndex = dvChannel.Find(drChunk["userID"].ToString());
								if (rowIndex == -1)
								{
									WriteLogEntry("error", "Invalid Channel : " + thisFilePath);
								}
								else
								{
									int channelID = Int32.Parse(dvChannel[rowIndex]["id"].ToString());
									int fps = Int32.Parse(dvChannel[rowIndex]["fps"].ToString());
									DateTime start_tc = new LinearTimeCode(fps, drChunk["startDate"].ToString(), drChunk["startTimeCode"].ToString()).dt;
									DateTime end_tc = new LinearTimeCode(fps, drChunk["startDate"].ToString(), drChunk["startTimeCode"].ToString()).dt;
									long duration = ConvertToFrames(drChunk["fileDuration"].ToString(),fps);
									int offset = 0;
									int isblank = 0;
									String url = urlPrefix + drChunk["fileName"].ToString() + ".mpg";
									mwConnection.Open();
									SqlCommand insCmd = new SqlCommand("dbo.InsertChunk",mwConnection);
									insCmd.CommandType = CommandType.StoredProcedure;
									insCmd.Parameters.Add("@start_tc", SqlDbType.DateTime, 8);
									insCmd.Parameters["@start_tc"].Value = start_tc;
									insCmd.Parameters.Add("@channel_id", SqlDbType.Int,4);
									insCmd.Parameters["@channel_id"].Value = channelID;
									insCmd.Parameters.Add("@duration", SqlDbType.Int, 8);
									insCmd.Parameters["@duration"].Value = duration;
									insCmd.Parameters.Add("@offset", SqlDbType.Int, 4);
									insCmd.Parameters["@offset"].Value = offset;
									insCmd.Parameters.Add("@isblank", SqlDbType.TinyInt);
									insCmd.Parameters["@isblank"].Value = isblank;
									insCmd.Parameters.Add("@url", SqlDbType.NVarChar, 512);
									insCmd.Parameters["@url"].Value = url;
									insCmd.ExecuteNonQuery();
									mwConnection.Close();
									insCmd.Dispose();
									WriteLogEntry("info",String.Format("dbo.InsertChunk {0},{1},{2}",start_tc.ToString(),channelID.ToString(),url));
								}
							}
							if ((numberOfChunkFiles < 64) || (numberOfChunkFiles % 64 == 0)) 
							{
								messageListBox.Items.Add("INFO: Chunk Watcher at point " + numberOfChunkFiles.ToString());
							}
						}
						catch (System.IO.FileNotFoundException ex) 
						{
							WriteLogEntry("error",ex.ToString());
						}
						catch (System.IO.IOException ex)
						{
							WriteLogEntry("error",ex.ToString());
						}
						catch (System.ArgumentException ex)
						{
							WriteLogEntry("error",ex.ToString());
						}
						catch (System.Data.SqlClient.SqlException ex) 
						{
							WriteLogEntry("error",ex.ToString());
						}
						finally 
						{
							mwConnection.Close();
						}
					}
				}
			} while (numFilesInThisPass > 0);
		}

		private int ConvertToFrames(String fileDuration, int fps)
		{

			// The assumption at the moment is:
			// I will just do FF + (SS + MM * 60 + HH * 3600) * FPS
			
			int hh = Int32.Parse(fileDuration.Substring(0,2));
			int mm = Int32.Parse(fileDuration.Substring(3,2));
			int ss = Int32.Parse(fileDuration.Substring(6,2));
			int ff = Int32.Parse(fileDuration.Substring(9,2));

			return(ff+(ss+mm*60+hh*3600)*fps);
		}

		private void NewAsrunEvent(object sender, System.IO.FileSystemEventArgs e)
		{
			int numFilesInThisPass = 0;
			do 
			{
				DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(e.FullPath));
				// Create an array representing the files in the current directory.
				FileInfo[] fi = di.GetFiles();
				foreach (FileInfo fiTemp in fi) 
				{
					if (fiTemp.Extension.Equals(".xml")) 
					{
						string thisFilePath = fiTemp.FullName;
						DataSet dsAsrun = new DataSet("asrun");
						int fps;
						try
						{
							dsAsrun.ReadXml(thisFilePath,XmlReadMode.InferSchema);
							// TODO: Look at good way of getting round this.
							// currently user is Local Service, which can't delete files.
							File.Delete(thisFilePath);
							archiveDataset(dsAsrun);
							numberOfAsrunFiles++;
							numFilesInThisPass++;
							// Look up channel name against dsChannels dataset
							DataView dvChannel = new DataView(dsChannels.Tables["channels"], "",
								"name", DataViewRowState.CurrentRows);
							DataTable dtAsrun = dsAsrun.Tables["AsRun"];
							foreach (DataRow drAsrun in dtAsrun.Rows) 
							{ // There can be more than one row entered in one file on asrun log.
								int rowIndex = dvChannel.Find(drAsrun["channel"].ToString());
								if (rowIndex == -1) 
								{
									WriteLogEntry("error", "Invalid Channel : " + thisFilePath);
								} 
								else
								{
									int channel_id = Int32.Parse(dvChannel[rowIndex]["id"].ToString());
									fps = Int32.Parse(dvChannel[rowIndex]["fps"].ToString());
									DateTime start_tc = new LinearTimeCode(fps,
										drAsrun["startDate"].ToString(),
										drAsrun["startTimeCode"].ToString()).dt;
									DateTime end_tc = new LinearTimeCode(fps,
										drAsrun["startDate"].ToString(),
										drAsrun["startTimeCode"].ToString()).dt;
									long duration = ConvertToFrames(drAsrun["duration"].ToString(),fps);
									int offset = 0; // The offset will be added later.
									int isblank = 0; // Not blank, obviously!
									string id = drAsrun["id"].ToString();
									string title = drAsrun["title"].ToString();
									string s = drAsrun["s"].ToString();
									string status = drAsrun["status"].ToString();
									string ch = drAsrun["ch"].ToString();
									string reconcile = drAsrun["reconcile"].ToString();
									string ty = drAsrun["ty"].ToString();
									// Execute the stored procedure with these parameters.
									// @start_tc datetime, @channel_id int, @end_tc datetime, @duration bigint,
									// @offset int, @isblank bit, @id varchar(64), @title varchar(64), @s varchar(64),
									// @status varchar(64), @ch varchar(64), @reconcile varchar(64), @ty varchar(64)
									mwConnection.Open();
									SqlCommand insCmd = new SqlCommand("dbo.InsertAsrun",mwConnection);
									insCmd.CommandType = CommandType.StoredProcedure;
									insCmd.Parameters.Add("@start_tc", SqlDbType.DateTime, 8);
									insCmd.Parameters["@start_tc"].Value = start_tc;
									insCmd.Parameters.Add("@channel_id", SqlDbType.Int,4);
									insCmd.Parameters["@channel_id"].Value = channel_id;
									insCmd.Parameters.Add("@duration", SqlDbType.Int, 8);
									insCmd.Parameters["@duration"].Value = duration;
									insCmd.Parameters.Add("@offset", SqlDbType.Int, 4);
									insCmd.Parameters["@offset"].Value = offset;
									insCmd.Parameters.Add("@isblank", SqlDbType.TinyInt);
									insCmd.Parameters["@isblank"].Value = isblank;
									insCmd.Parameters.Add("@id", SqlDbType.NVarChar, 64);
									insCmd.Parameters["@id"].Value = id;
									insCmd.Parameters.Add("@title", SqlDbType.NVarChar, 64);
									insCmd.Parameters["@title"].Value = title;
									insCmd.Parameters.Add("@s", SqlDbType.NVarChar, 64);
									insCmd.Parameters["@s"].Value = s;
									insCmd.Parameters.Add("@status", SqlDbType.NVarChar, 64);
									insCmd.Parameters["@status"].Value = status;
									insCmd.Parameters.Add("@ch", SqlDbType.NVarChar, 64);
									insCmd.Parameters["@ch"].Value = ch;
									insCmd.Parameters.Add("@reconcile", SqlDbType.NVarChar, 64);
									insCmd.Parameters["@reconcile"].Value = reconcile;
									insCmd.Parameters.Add("@ty", SqlDbType.NVarChar, 64);
									insCmd.Parameters["@ty"].Value = ty;
									insCmd.ExecuteNonQuery();
									mwConnection.Close();
									insCmd.Dispose();
									WriteLogEntry("info",String.Format("dbo.InsertAsrun {0},{1},{2}",start_tc.ToString(),channel_id.ToString(),title));
									if ((numberOfAsrunFiles < 64) || (numberOfAsrunFiles % 64 == 0)) 
									{
										messageListBox.Items.Add("INFO: Asrun Watcher at point " + numberOfAsrunFiles.ToString());
									}								
								}

							}
						}
						catch (System.IO.IOException ex)
						{
							WriteLogEntry("error", ex.ToString());
						}
						catch (System.ArgumentException ex)
						{
							WriteLogEntry("error",ex.ToString());
						}
						catch (SqlException ex) 
						{
							WriteLogEntry("error",ex.ToString());
						}
						finally
						{
							mwConnection.Close();
						}
					}
				}
			} while (numFilesInThisPass > 0);
		}

		/// <summary>
		/// Archive Dataset to log file. Useful to reload dataset.
		/// </summary>
		/// <param name="thisDs">The dataset to archive.</param>
		private void archiveDataset(DataSet thisDs) 
		{
//			if (!File.Exists(logFile)) 
//			{
//				thisDs.WriteXml(logFile,XmlWriteMode.IgnoreSchema);
//			} 
//			else 
//			{
//				System.IO.FileStream logFileStream = new System.IO.FileStream
//					(logFile, System.IO.FileMode.Append);
//				// Write to the file with the WriteXml method.
//				thisDs.WriteXml(logFileStream,XmlWriteMode.IgnoreSchema);
//				logFileStream.Close();
//			}
		}

		/// <summary>
		/// Write Entry to log file
		/// </summary>
		/// <param name="logtype">log type, typically info or error</param>
		/// <param name="logmessage">log message.</param>
		private void WriteLogEntry(String logtype, String logmessage)
		{
			StreamWriter lf = File.AppendText(logFile);
			lf.WriteLine(logtype + " : " + logmessage);
			lf.Close();
		}
	}
}

