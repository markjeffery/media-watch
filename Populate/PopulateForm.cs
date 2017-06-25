using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace Populate
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class populateForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label numDaysLabel;
		private System.Windows.Forms.Label numChannelsLabel;
		private System.Windows.Forms.ComboBox numChannelsCombo;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox programmeMixCombo;
		private System.Windows.Forms.ListBox programmeMixListBox;
		private System.Windows.Forms.Button addButton;
		private System.Windows.Forms.Button deleteButton;
		private System.Windows.Forms.ProgressBar insertAsrunProgressBar;
		private System.Windows.Forms.Label lblProgressAsrun;
		private System.Windows.Forms.ProgressBar insertChunkProgressBar;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Button btnPopulate;
		private System.Windows.Forms.Label lblProgressChunk;
		private System.Windows.Forms.Label lblStatus;
		private System.Windows.Forms.ComboBox numDaysCombo;

		private int numRecsChunk;
		private int numRecsAsRun;
		private int numDays;
		private int numChannels;
		private int ChunkSize;
		private int[] proglist;
		private System.Windows.Forms.ComboBox chunkSizeCombo;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public populateForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			updateLabels();
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
		
		private void updateLabels()
		{
			numDays = Int32.Parse( numDaysCombo.SelectedItem.ToString());
			numChannels = Int32.Parse( numChannelsCombo.SelectedItem.ToString());
			ChunkSize = Int32.Parse( chunkSizeCombo.SelectedItem.ToString());
			int numProgs = 0;
			int lengthProgs = 0;
			proglist = new int[programmeMixListBox.Items.Count];
			IEnumerator listEnum = programmeMixListBox.Items.GetEnumerator();
			int i=0;
			while ( listEnum.MoveNext() ) 
			{
				numProgs++;
				lengthProgs += Int32.Parse( listEnum.Current.ToString() );
				proglist[i++] = Int32.Parse( listEnum.Current.ToString() );
			}

			numRecsChunk = numDays * numChannels * (int) (1440 / ChunkSize);
			numRecsAsRun = numProgs * numChannels * (int) ((86400 * numDays) / lengthProgs);
			lblStatus.Text = "chunk Table: " + numRecsChunk.ToString() + " rows, asrun table " + numRecsAsRun.ToString() + " rows";
			lblProgressChunk.Text = "No data inserted into chunk table yet.";
			lblProgressAsrun.Text = "No data inserted into chunk table yet.";
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.numDaysLabel = new System.Windows.Forms.Label();
			this.numDaysCombo = new System.Windows.Forms.ComboBox();
			this.numChannelsLabel = new System.Windows.Forms.Label();
			this.numChannelsCombo = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.chunkSizeCombo = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.programmeMixCombo = new System.Windows.Forms.ComboBox();
			this.programmeMixListBox = new System.Windows.Forms.ListBox();
			this.addButton = new System.Windows.Forms.Button();
			this.deleteButton = new System.Windows.Forms.Button();
			this.insertAsrunProgressBar = new System.Windows.Forms.ProgressBar();
			this.lblProgressAsrun = new System.Windows.Forms.Label();
			this.insertChunkProgressBar = new System.Windows.Forms.ProgressBar();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnPopulate = new System.Windows.Forms.Button();
			this.lblProgressChunk = new System.Windows.Forms.Label();
			this.lblStatus = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// numDaysLabel
			// 
			this.numDaysLabel.Location = new System.Drawing.Point(32, 32);
			this.numDaysLabel.Name = "numDaysLabel";
			this.numDaysLabel.Size = new System.Drawing.Size(120, 23);
			this.numDaysLabel.TabIndex = 0;
			this.numDaysLabel.Text = "Number of Days";
			// 
			// numDaysCombo
			// 
			this.numDaysCombo.Items.AddRange(new object[] {
															  "7",
															  "30",
															  "90",
															  "360"});
			this.numDaysCombo.Location = new System.Drawing.Point(168, 32);
			this.numDaysCombo.Name = "numDaysCombo";
			this.numDaysCombo.Size = new System.Drawing.Size(121, 21);
			this.numDaysCombo.TabIndex = 1;
			this.numDaysCombo.Text = "90";
			this.numDaysCombo.SelectedIndexChanged += new System.EventHandler(this.numDaysCombo_SelectedIndexChanged);
			// 
			// numChannelsLabel
			// 
			this.numChannelsLabel.Location = new System.Drawing.Point(32, 80);
			this.numChannelsLabel.Name = "numChannelsLabel";
			this.numChannelsLabel.Size = new System.Drawing.Size(128, 23);
			this.numChannelsLabel.TabIndex = 2;
			this.numChannelsLabel.Text = "Number of Channels";
			// 
			// numChannelsCombo
			// 
			this.numChannelsCombo.Items.AddRange(new object[] {
																  "1",
																  "4",
																  "8",
																  "32",
																  "128"});
			this.numChannelsCombo.Location = new System.Drawing.Point(168, 72);
			this.numChannelsCombo.Name = "numChannelsCombo";
			this.numChannelsCombo.Size = new System.Drawing.Size(121, 21);
			this.numChannelsCombo.TabIndex = 3;
			this.numChannelsCombo.Text = "4";
			this.numChannelsCombo.SelectedIndexChanged += new System.EventHandler(this.numChannelsCombo_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(32, 120);
			this.label1.Name = "label1";
			this.label1.TabIndex = 4;
			this.label1.Text = "Chunk Size (mins)";
			// 
			// chunkSizeCombo
			// 
			this.chunkSizeCombo.Items.AddRange(new object[] {
																"3",
																"10",
																"30",
																"60"});
			this.chunkSizeCombo.Location = new System.Drawing.Point(168, 112);
			this.chunkSizeCombo.Name = "chunkSizeCombo";
			this.chunkSizeCombo.Size = new System.Drawing.Size(121, 21);
			this.chunkSizeCombo.TabIndex = 5;
			this.chunkSizeCombo.Text = "3";
			this.chunkSizeCombo.SelectedIndexChanged += new System.EventHandler(this.chunkSizeCombo_SelectedIndexChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(32, 160);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(128, 23);
			this.label2.TabIndex = 6;
			this.label2.Text = "programmeMixLabel";
			// 
			// programmeMixCombo
			// 
			this.programmeMixCombo.Items.AddRange(new object[] {
																   "45",
																   "90",
																   "600",
																   "1200",
																   "2400",
																   "3600"});
			this.programmeMixCombo.Location = new System.Drawing.Point(168, 160);
			this.programmeMixCombo.Name = "programmeMixCombo";
			this.programmeMixCombo.Size = new System.Drawing.Size(121, 21);
			this.programmeMixCombo.TabIndex = 7;
			this.programmeMixCombo.Text = "90";
			// 
			// programmeMixListBox
			// 
			this.programmeMixListBox.Items.AddRange(new object[] {
																	 "90",
																	 "90",
																	 "90",
																	 "90",
																	 "90",
																	 "90",
																	 "90",
																	 "600",
																	 "90",
																	 "90",
																	 "90",
																	 "90",
																	 "90",
																	 "90",
																	 "600",
																	 "90",
																	 "90",
																	 "90",
																	 "90",
																	 "90",
																	 "90",
																	 "600"});
			this.programmeMixListBox.Location = new System.Drawing.Point(168, 192);
			this.programmeMixListBox.Name = "programmeMixListBox";
			this.programmeMixListBox.Size = new System.Drawing.Size(120, 134);
			this.programmeMixListBox.TabIndex = 8;
			this.programmeMixListBox.SelectedIndexChanged += new System.EventHandler(this.programmeMixListBox_SelectedIndexChanged);
			// 
			// addButton
			// 
			this.addButton.Location = new System.Drawing.Point(312, 160);
			this.addButton.Name = "addButton";
			this.addButton.TabIndex = 9;
			this.addButton.Text = "Add";
			this.addButton.Click += new System.EventHandler(this.addButton_Click);
			// 
			// deleteButton
			// 
			this.deleteButton.Enabled = false;
			this.deleteButton.Location = new System.Drawing.Point(312, 192);
			this.deleteButton.Name = "deleteButton";
			this.deleteButton.TabIndex = 10;
			this.deleteButton.Text = "Delete";
			this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
			// 
			// insertAsrunProgressBar
			// 
			this.insertAsrunProgressBar.Location = new System.Drawing.Point(24, 464);
			this.insertAsrunProgressBar.Name = "insertAsrunProgressBar";
			this.insertAsrunProgressBar.Size = new System.Drawing.Size(376, 23);
			this.insertAsrunProgressBar.TabIndex = 24;
			// 
			// lblProgressAsrun
			// 
			this.lblProgressAsrun.Location = new System.Drawing.Point(24, 440);
			this.lblProgressAsrun.Name = "lblProgressAsrun";
			this.lblProgressAsrun.Size = new System.Drawing.Size(376, 23);
			this.lblProgressAsrun.TabIndex = 23;
			// 
			// insertChunkProgressBar
			// 
			this.insertChunkProgressBar.Location = new System.Drawing.Point(24, 408);
			this.insertChunkProgressBar.Name = "insertChunkProgressBar";
			this.insertChunkProgressBar.Size = new System.Drawing.Size(376, 23);
			this.insertChunkProgressBar.TabIndex = 22;
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(64, 504);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.TabIndex = 21;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// btnPopulate
			// 
			this.btnPopulate.Location = new System.Drawing.Point(264, 504);
			this.btnPopulate.Name = "btnPopulate";
			this.btnPopulate.TabIndex = 20;
			this.btnPopulate.Text = "Populate";
			this.btnPopulate.Click += new System.EventHandler(this.btnPopulate_Click);
			// 
			// lblProgressChunk
			// 
			this.lblProgressChunk.Location = new System.Drawing.Point(24, 384);
			this.lblProgressChunk.Name = "lblProgressChunk";
			this.lblProgressChunk.Size = new System.Drawing.Size(376, 23);
			this.lblProgressChunk.TabIndex = 19;
			// 
			// lblStatus
			// 
			this.lblStatus.Location = new System.Drawing.Point(24, 352);
			this.lblStatus.Name = "lblStatus";
			this.lblStatus.Size = new System.Drawing.Size(376, 23);
			this.lblStatus.TabIndex = 18;
			this.lblStatus.Text = "Change Parameters, and click on Populate.";
			// 
			// populateForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(432, 573);
			this.Controls.Add(this.insertAsrunProgressBar);
			this.Controls.Add(this.lblProgressAsrun);
			this.Controls.Add(this.insertChunkProgressBar);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnPopulate);
			this.Controls.Add(this.lblProgressChunk);
			this.Controls.Add(this.lblStatus);
			this.Controls.Add(this.deleteButton);
			this.Controls.Add(this.addButton);
			this.Controls.Add(this.programmeMixListBox);
			this.Controls.Add(this.programmeMixCombo);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.chunkSizeCombo);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.numChannelsCombo);
			this.Controls.Add(this.numChannelsLabel);
			this.Controls.Add(this.numDaysCombo);
			this.Controls.Add(this.numDaysLabel);
			this.Name = "populateForm";
			this.Text = "Populate";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new populateForm());
		}

		private void programmeMixListBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (programmeMixListBox.SelectedIndices.Count > 0)
			{
				deleteButton.Enabled = true;
			} 
			else
			{
				deleteButton.Enabled = false;
			}
		}

		private void deleteButton_Click(object sender, System.EventArgs e)
		{
			programmeMixListBox.Items.RemoveAt(programmeMixListBox.SelectedIndex);

			updateLabels();
		}

		private void addButton_Click(object sender, System.EventArgs e)
		{
			programmeMixListBox.Items.Add(programmeMixCombo.SelectedItem);

			updateLabels();
		}

		private void numDaysCombo_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			updateLabels();
		}

		private void numChannelsCombo_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			updateLabels();
		}

		private void chunkSizeCombo_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			updateLabels();
		}

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		delegate void ShowProgressAsRunDelegate(int recSoFar, int totalRecs);

		void ShowProgressAsRun(int recsSoFar, int totalRecs)
		{
			if (btnPopulate.InvokeRequired == false)
			{
				lblProgressAsrun.Text = "Number of rows inserted: " + recsSoFar.ToString() + " of " + totalRecs.ToString();
				insertAsrunProgressBar.Maximum = totalRecs;
				insertAsrunProgressBar.Value = recsSoFar;
			}
			else
			{
				ShowProgressAsRunDelegate showProgressAsRun = new ShowProgressAsRunDelegate(ShowProgressAsRun);
				BeginInvoke(showProgressAsRun, new object[] {recsSoFar, totalRecs});
			}
		}

		// Delegate stuff to do the insert as a thread.
		delegate void ShowProgressChunkDelegate(int recsSoFar, int totalRecs);

		void ShowProgressChunk(int recsSoFar, int totalRecs) 
		{
			if (btnPopulate.InvokeRequired == false) 
			{
				lblProgressChunk.Text = "Number of rows inserted: " + recsSoFar.ToString() + " of " + totalRecs.ToString();
				insertChunkProgressBar.Maximum = totalRecs;
				insertChunkProgressBar.Value = recsSoFar;
			}
			else
			{
				ShowProgressChunkDelegate showProgressChunk = new ShowProgressChunkDelegate(ShowProgressChunk);
				BeginInvoke(showProgressChunk, new object[] {recsSoFar, totalRecs});
			}
		}

		void PopulateChunk() 
		{
			int numRecords = 0;
			int numRecordsProcessed = 0;
			ChunkDataset cs = new ChunkDataset();
			cs.ChunkSize = ChunkSize;
			cs.NumChannels = numChannels;
			cs.NumDays = numDays;
			cs.StartPopulate();
			ShowProgressChunk(0,numRecsChunk);
			while (numRecordsProcessed >= 0)
			{
				numRecordsProcessed = cs.Populate();
				if (numRecordsProcessed >= 0) 
				{
					numRecords += numRecordsProcessed;
					ShowProgressChunk(numRecords, numRecsChunk);
				}
				else
				{
					ShowProgressChunk(numRecsChunk, numRecsChunk);
				}
			}
		}


		delegate void PopulateChunkDelegate();

		void PopulateAsRun()
		{
			int numRecords = 0;
			int numRecordsProcessed = 0;
			AsRunDataset asr = new AsRunDataset();
			asr.NumChannels = numChannels;
			asr.NumDays = numDays;
			asr.ProgList = proglist;
			asr.StartPopulate();
			ShowProgressAsRun(0,numRecsAsRun);
			while (numRecordsProcessed >= 0)
			{
				numRecordsProcessed = asr.Populate();	
				if (numRecordsProcessed >= 0) 
				{
					numRecords += numRecordsProcessed;
					ShowProgressAsRun(numRecords, numRecsAsRun);
				}
				else
				{
					ShowProgressAsRun(numRecsAsRun, numRecsAsRun);
				}
			}
		}

		private void btnPopulate_Click(object sender, System.EventArgs e)
		{
			PopulateChunk();
			PopulateAsRun();
		}


	}
}
