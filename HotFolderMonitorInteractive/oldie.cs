using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace HotFolderMonitor
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListBox newItemListBox;
		private System.IO.FileSystemWatcher chunkFileSystemWatcher;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private DataSet dsAssetOut;
		private System.Data.SqlClient.SqlConnection sqlConnection1;
		private DataSet dsChannels;
		private Queue chunkQueue = new Queue();
		private int numberOfFiles = 0;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			label1.Text = "Watching: " + chunkFileSystemWatcher.Path;

			// Create dataset for output data.
			dsAssetOut = new DataSet("asset-out");
			DataTable chunkTable = dsAssetOut.Tables.Add("chunks");
			DataColumn start_dtCol    = chunkTable.Columns.Add("start_dt", typeof(DateTime));
			DataColumn start_frameCol = chunkTable.Columns.Add("start_frame", typeof(Int32));
			DataColumn channelCol     = chunkTable.Columns.Add("channel", typeof(String));
			DataColumn end_dtCol      = chunkTable.Columns.Add("end_dt", typeof(DateTime));
			DataColumn end_frameCol   = chunkTable.Columns.Add("end_frame", typeof(Int32));
			DataColumn durationCol    = chunkTable.Columns.Add("duration", typeof(Int64));
			DataColumn offsetCol      = chunkTable.Columns.Add("offset", typeof(Int64));
			DataColumn isblankCol     = chunkTable.Columns.Add("isblank", typeof(Boolean));
			DataColumn urlCol         = chunkTable.Columns.Add("url", typeof(String));
			chunkTable.PrimaryKey = new DataColumn[] {start_dtCol, start_frameCol, channelCol};

			// Create dataset for channels.
			dsChannels = new DataSet("channels");
			String sql = "select * from channels";
			sqlConnection1.Open();
			SqlDataAdapter daChannel = new SqlDataAdapter(sql, sqlConnection1);
			daChannel.Fill(dsChannels, "channels");
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.newItemListBox = new System.Windows.Forms.ListBox();
			this.chunkFileSystemWatcher = new System.IO.FileSystemWatcher();
			this.sqlConnection1 = new System.Data.SqlClient.SqlConnection();
			((System.ComponentModel.ISupportInitialize)(this.chunkFileSystemWatcher)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(624, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Watching: ";
			// 
			// newItemListBox
			// 
			this.newItemListBox.Location = new System.Drawing.Point(24, 64);
			this.newItemListBox.Name = "newItemListBox";
			this.newItemListBox.ScrollAlwaysVisible = true;
			this.newItemListBox.Size = new System.Drawing.Size(616, 186);
			this.newItemListBox.TabIndex = 1;
			// 
			// chunkFileSystemWatcher
			// 
			this.chunkFileSystemWatcher.EnableRaisingEvents = true;
			this.chunkFileSystemWatcher.Filter = "*.xml";
			this.chunkFileSystemWatcher.Path = "C:\\Documents and Settings\\Administrator\\My Documents\\test hot";
			this.chunkFileSystemWatcher.SynchronizingObject = this;
			this.chunkFileSystemWatcher.Created += new System.IO.FileSystemEventHandler(this.NewChunkEvent);
			// 
			// sqlConnection1
			// 
			this.sqlConnection1.ConnectionString = "workstation id=\"SONY-PDS1L39IGJ\";packet size=4096;integrated security=SSPI;data s" +
				"ource=\"(local)\\NetSDK\";persist security info=False;initial catalog=MediaWatch";
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(664, 273);
			this.Controls.Add(this.newItemListBox);
			this.Controls.Add(this.label1);
			this.Name = "Form1";
			this.Text = "This hot folder watcher will be a service!";
			((System.ComponentModel.ISupportInitialize)(this.chunkFileSystemWatcher)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Form1());		
		}

		private void NewChunkEvent(object sender, System.IO.FileSystemEventArgs e)
		{
			DataSet dsAssets = new DataSet("asset");
			DataRow drout;
			string filePath = e.FullPath.ToString();
			// Put filename into queue so that we get another chance
			// at a file, if it is locked.
			chunkQueue.Enqueue(e.FullPath.ToString());
			while (chunkQueue.Count > 0) 
			{
				string thisFilePath = chunkQueue.Dequeue().ToString();
				try
				{
					dsAssets.ReadXml(thisFilePath);
					numberOfFiles++;
					Console.WriteLine("Processing " + numberOfFiles.ToString() + ":" + thisFilePath);
					string channelName = dsAssets.Tables["Asset"].Rows[0]["userID"].ToString();
					DataView dvChannel = new DataView();
					dvChannel.Table = dsChannels.Tables["channels"];
					dvChannel.RowFilter = "name = '" + channelName + "'";
					string channelID = dvChannel[0]["id"].ToString();
					string fileName = dsAssets.Tables["Asset"].Rows[0]["fileName"].ToString();
					string fileDuration = dsAssets.Tables["Asset"].Rows[0]["fileDuration"].ToString();
					string startDate = dsAssets.Tables["Asset"].Rows[0]["startDate"].ToString();
					string startTimeCode = dsAssets.Tables["Asset"].Rows[0]["startTimeCode"].ToString();
					newItemListBox.Items.Add(channelID + "," + channelName + "," + startDate + "," + startTimeCode + "," + fileDuration + "," + fileName);
					drout = dsAssetOut.Tables["chunks"].NewRow();
					drout["start_dt"] = new DateTime(
					DataColumn start_dtCol    = chunkTable.Columns.Add("start_dt", typeof(DateTime));
					DataColumn start_frameCol = chunkTable.Columns.Add("start_frame", typeof(Int32));
					DataColumn channelCol     = chunkTable.Columns.Add("channel", typeof(String));
					DataColumn end_dtCol      = chunkTable.Columns.Add("end_dt", typeof(DateTime));
					DataColumn end_frameCol   = chunkTable.Columns.Add("end_frame", typeof(Int32));
					DataColumn durationCol    = chunkTable.Columns.Add("duration", typeof(Int64));
					DataColumn offsetCol      = chunkTable.Columns.Add("offset", typeof(Int64));
					DataColumn isblankCol     = chunkTable.Columns.Add("isblank", typeof(Boolean));
					DataColumn urlCol         = chunkTable.Columns.Add("url", typeof(String));

					workRow = workTable.NewRow();
					workRow[0] = i;
					workRow[1] = "CustName" + i.ToString();
					workTable.Rows.Add(workRow);
					// todo: move data over from dsAssets to dsAssetsout, and write to database.
					//       change database schema to reflect differences.
				}
				catch (System.IO.IOException ex)
				{
					Console.WriteLine(ex.ToString());
					Console.WriteLine("Will Queue up " + thisFilePath + " for later");
					chunkQueue.Enqueue(thisFilePath);
				}
				catch (System.ArgumentException ex)
				{
					Console.WriteLine("Problem with " + thisFilePath + ":" + ex.ToString());
					// Don't re enqueue this one, as this is a problem that won't go away!
				}

			}
		}
	}
}
