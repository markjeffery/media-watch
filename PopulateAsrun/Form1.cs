using System;
using System.Drawing;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Windows.Forms;

namespace PopulateAsrun
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Form1 : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ListBox mixListBox;
		private System.Windows.Forms.TextBox addDurTextBox;
		private System.Windows.Forms.Label addToMixLabel;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button delTitleButton;
		private System.Windows.Forms.Button addTitleButton;
		private System.Windows.Forms.ListBox titleListBox;
		private System.Windows.Forms.TextBox addTitleTextBox;
		private System.Windows.Forms.Label addTitleLabel;
		private System.Windows.Forms.Label statusLabel;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button populateButton;
		private System.Windows.Forms.Button delDurButton;
		private System.Windows.Forms.Button addDurButton;
		private System.Windows.Forms.Timer timer1;
		private System.Data.SqlClient.SqlConnection mwSqlConnection;
		private System.ComponentModel.IContainer components;

		private int channelIndex = 0;
		private DateTime currentTimecode;
		private DateTime thisMax;
		private int indexDuration = 0;
		private int indexTitle = 0;
		private System.Data.SqlClient.SqlCommand insertAsrunSqlCommand;
		private System.Windows.Forms.Label exceptionLabel;
		private int[] channels;

		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			this.components = new System.ComponentModel.Container();
			System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.delDurButton = new System.Windows.Forms.Button();
			this.addDurButton = new System.Windows.Forms.Button();
			this.mixListBox = new System.Windows.Forms.ListBox();
			this.addDurTextBox = new System.Windows.Forms.TextBox();
			this.addToMixLabel = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.delTitleButton = new System.Windows.Forms.Button();
			this.addTitleButton = new System.Windows.Forms.Button();
			this.titleListBox = new System.Windows.Forms.ListBox();
			this.addTitleTextBox = new System.Windows.Forms.TextBox();
			this.addTitleLabel = new System.Windows.Forms.Label();
			this.statusLabel = new System.Windows.Forms.Label();
			this.cancelButton = new System.Windows.Forms.Button();
			this.populateButton = new System.Windows.Forms.Button();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.mwSqlConnection = new System.Data.SqlClient.SqlConnection();
			this.insertAsrunSqlCommand = new System.Data.SqlClient.SqlCommand();
			this.exceptionLabel = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(648, 40);
			this.label1.TabIndex = 0;
			this.label1.Text = "This form will populate the asrun table for the same time period as the chunk tab" +
				"le.";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.delDurButton);
			this.groupBox1.Controls.Add(this.addDurButton);
			this.groupBox1.Controls.Add(this.mixListBox);
			this.groupBox1.Controls.Add(this.addDurTextBox);
			this.groupBox1.Controls.Add(this.addToMixLabel);
			this.groupBox1.Location = new System.Drawing.Point(24, 88);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(296, 264);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Duration Mix";
			// 
			// delDurButton
			// 
			this.delDurButton.Enabled = false;
			this.delDurButton.Location = new System.Drawing.Point(184, 120);
			this.delDurButton.Name = "delDurButton";
			this.delDurButton.TabIndex = 9;
			this.delDurButton.Text = "Delete";
			this.delDurButton.Click += new System.EventHandler(this.delDurButton_Click);
			// 
			// addDurButton
			// 
			this.addDurButton.Location = new System.Drawing.Point(184, 72);
			this.addDurButton.Name = "addDurButton";
			this.addDurButton.TabIndex = 8;
			this.addDurButton.Text = "Add";
			this.addDurButton.Click += new System.EventHandler(this.addDurButton_Click);
			// 
			// mixListBox
			// 
			this.mixListBox.ItemHeight = 16;
			this.mixListBox.Items.AddRange(new object[] {
															"90",
															"90",
															"90",
															"90",
															"2400",
															"90",
															"90",
															"90",
															"90",
															"2400",
															"90",
															"90",
															"90",
															"90"});
			this.mixListBox.Location = new System.Drawing.Point(32, 72);
			this.mixListBox.Name = "mixListBox";
			this.mixListBox.ScrollAlwaysVisible = true;
			this.mixListBox.Size = new System.Drawing.Size(120, 180);
			this.mixListBox.TabIndex = 7;
			this.mixListBox.SelectedIndexChanged += new System.EventHandler(this.mixListBox_SelectedIndexChanged);
			// 
			// addDurTextBox
			// 
			this.addDurTextBox.Location = new System.Drawing.Point(176, 32);
			this.addDurTextBox.Name = "addDurTextBox";
			this.addDurTextBox.TabIndex = 6;
			this.addDurTextBox.Text = "90";
			// 
			// addToMixLabel
			// 
			this.addToMixLabel.Location = new System.Drawing.Point(16, 32);
			this.addToMixLabel.Name = "addToMixLabel";
			this.addToMixLabel.Size = new System.Drawing.Size(128, 23);
			this.addToMixLabel.TabIndex = 5;
			this.addToMixLabel.Text = "Add Duration to Mix";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.delTitleButton);
			this.groupBox2.Controls.Add(this.addTitleButton);
			this.groupBox2.Controls.Add(this.titleListBox);
			this.groupBox2.Controls.Add(this.addTitleTextBox);
			this.groupBox2.Controls.Add(this.addTitleLabel);
			this.groupBox2.Location = new System.Drawing.Point(336, 88);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(304, 264);
			this.groupBox2.TabIndex = 3;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Title Mix";
			// 
			// delTitleButton
			// 
			this.delTitleButton.Enabled = false;
			this.delTitleButton.Location = new System.Drawing.Point(190, 106);
			this.delTitleButton.Name = "delTitleButton";
			this.delTitleButton.TabIndex = 14;
			this.delTitleButton.Text = "Delete";
			this.delTitleButton.Click += new System.EventHandler(this.delTitleButton_Click);
			// 
			// addTitleButton
			// 
			this.addTitleButton.Location = new System.Drawing.Point(190, 58);
			this.addTitleButton.Name = "addTitleButton";
			this.addTitleButton.TabIndex = 13;
			this.addTitleButton.Text = "Add";
			this.addTitleButton.Click += new System.EventHandler(this.addTitleButton_Click);
			// 
			// titleListBox
			// 
			this.titleListBox.ItemHeight = 16;
			this.titleListBox.Items.AddRange(new object[] {
															  "Coronation Street",
															  "Emmerdale",
															  "Trisha Extra",
															  "Pop Idol Extra",
															  "Trisha",
															  "The John Walsh Show",
															  "Sally Jessy Raphael",
															  "Judge Judy",
															  "The Whole Nine Yards (2000)",
															  "The World\'s most Outrageous Weddings",
															  "David Letterman",
															  "Teleshopping",
															  "Movies Now",
															  "Little Antics",
															  "The Wheels on the bus",
															  "Elmo\'s World",
															  "Buzz and Poppy",
															  "Timbuctoo",
															  "Barney and Friends",
															  "Hi",
															  "Baby ER",
															  "Wellington: The Iron Duke",
															  "Eldorado",
															  "Are you being served?",
															  "The Flying Doctors",
															  "Lovejoy"});
			this.titleListBox.Location = new System.Drawing.Point(38, 58);
			this.titleListBox.Name = "titleListBox";
			this.titleListBox.ScrollAlwaysVisible = true;
			this.titleListBox.Size = new System.Drawing.Size(120, 180);
			this.titleListBox.TabIndex = 12;
			this.titleListBox.SelectedIndexChanged += new System.EventHandler(this.titleListBox_SelectedIndexChanged);
			// 
			// addTitleTextBox
			// 
			this.addTitleTextBox.Location = new System.Drawing.Point(182, 18);
			this.addTitleTextBox.Name = "addTitleTextBox";
			this.addTitleTextBox.TabIndex = 11;
			this.addTitleTextBox.Text = "90";
			// 
			// addTitleLabel
			// 
			this.addTitleLabel.Location = new System.Drawing.Point(22, 18);
			this.addTitleLabel.Name = "addTitleLabel";
			this.addTitleLabel.Size = new System.Drawing.Size(128, 23);
			this.addTitleLabel.TabIndex = 10;
			this.addTitleLabel.Text = "Add Title to Mix";
			// 
			// statusLabel
			// 
			this.statusLabel.Location = new System.Drawing.Point(24, 360);
			this.statusLabel.Name = "statusLabel";
			this.statusLabel.Size = new System.Drawing.Size(552, 23);
			this.statusLabel.TabIndex = 4;
			this.statusLabel.Text = "Status: Application started.";
			// 
			// cancelButton
			// 
			this.cancelButton.Location = new System.Drawing.Point(200, 472);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.TabIndex = 5;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// populateButton
			// 
			this.populateButton.Location = new System.Drawing.Point(392, 472);
			this.populateButton.Name = "populateButton";
			this.populateButton.TabIndex = 6;
			this.populateButton.Text = "Populate";
			this.populateButton.Click += new System.EventHandler(this.populateButton_Click);
			// 
			// timer1
			// 
			this.timer1.Interval = 5;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// mwSqlConnection
			// 
			this.mwSqlConnection.ConnectionString = ((string)(configurationAppSettings.GetValue("mwSqlConnection.ConnectionString", typeof(string))));
			// 
			// insertAsrunSqlCommand
			// 
			this.insertAsrunSqlCommand.CommandText = "dbo.[InsertAsrun]";
			this.insertAsrunSqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
			this.insertAsrunSqlCommand.Connection = this.mwSqlConnection;
			this.insertAsrunSqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@RETURN_VALUE", System.Data.SqlDbType.Int, 4, System.Data.ParameterDirection.ReturnValue, false, ((System.Byte)(0)), ((System.Byte)(0)), "", System.Data.DataRowVersion.Current, null));
			this.insertAsrunSqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@start_tc", System.Data.SqlDbType.DateTime, 8));
			this.insertAsrunSqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@channel_id", System.Data.SqlDbType.Int, 4));
			this.insertAsrunSqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@duration", System.Data.SqlDbType.BigInt, 8));
			this.insertAsrunSqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@offset", System.Data.SqlDbType.Int, 4));
			this.insertAsrunSqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@isblank", System.Data.SqlDbType.Bit, 1));
			this.insertAsrunSqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@id", System.Data.SqlDbType.VarChar, 64));
			this.insertAsrunSqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@title", System.Data.SqlDbType.VarChar, 64));
			this.insertAsrunSqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@s", System.Data.SqlDbType.VarChar, 64));
			this.insertAsrunSqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@status", System.Data.SqlDbType.VarChar, 64));
			this.insertAsrunSqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@ch", System.Data.SqlDbType.VarChar, 64));
			this.insertAsrunSqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@reconcile", System.Data.SqlDbType.VarChar, 64));
			this.insertAsrunSqlCommand.Parameters.Add(new System.Data.SqlClient.SqlParameter("@ty", System.Data.SqlDbType.VarChar, 64));
			// 
			// exceptionLabel
			// 
			this.exceptionLabel.Location = new System.Drawing.Point(24, 400);
			this.exceptionLabel.Name = "exceptionLabel";
			this.exceptionLabel.Size = new System.Drawing.Size(552, 56);
			this.exceptionLabel.TabIndex = 7;
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.ClientSize = new System.Drawing.Size(712, 539);
			this.Controls.Add(this.exceptionLabel);
			this.Controls.Add(this.populateButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.statusLabel);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label1);
			this.Name = "Form1";
			this.Text = "Populate asrun from chunk data";
			this.Load += new System.EventHandler(this.startup);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
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

		private void mixListBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (mixListBox.SelectedIndex == -1) 
			{
				delDurButton.Enabled = false;
			} 
			else 
			{
				delDurButton.Enabled = true;
			}
		}

		private void delDurButton_Click(object sender, System.EventArgs e)
		{
			mixListBox.Items.RemoveAt(mixListBox.SelectedIndex);
		}

		private void addDurButton_Click(object sender, System.EventArgs e)
		{
			mixListBox.Items.Add(addDurTextBox.Text);
		}

		private void titleListBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			delTitleButton.Enabled = (titleListBox.SelectedIndex != -1);
		}

		private void addTitleButton_Click(object sender, System.EventArgs e)
		{
			titleListBox.Items.Add(addTitleTextBox.Text);
		}

		private void delTitleButton_Click(object sender, System.EventArgs e)
		{
			titleListBox.Items.RemoveAt(titleListBox.SelectedIndex);
		}

		private void cancelButton_Click(object sender, System.EventArgs e)
		{
			this.Dispose();
		}

		private void startup(object sender, System.EventArgs e)
		{
		}

		private void timer1_Tick(object sender, System.EventArgs e)
		{
			try 
			{
				// Insert new record into populate database.
				if (currentTimecode > thisMax) 
				{
					channelIndex++;
					if (channelIndex >= channels.Length) 
					{
						timer1.Stop();
						populateButton.Enabled = true;
						return;
					} 
					else 
					{
						currentTimecode = getMinDate(channelIndex);
						thisMax = getMaxDate(channelIndex);
					}
				}
				indexDuration = (indexDuration+1) % mixListBox.Items.Count;
				indexTitle = (indexTitle+1) % titleListBox.Items.Count;
				insertAsrunSqlCommand.Parameters["@start_tc"].Value = currentTimecode;
				insertAsrunSqlCommand.Parameters["@channel_id"].Value = channels[channelIndex];
				insertAsrunSqlCommand.Parameters["@duration"].Value = 25 * int.Parse((string) mixListBox.Items[indexDuration]);
				insertAsrunSqlCommand.Parameters["@offset"].Value = 0;
				insertAsrunSqlCommand.Parameters["@isblank"].Value = 0;
				insertAsrunSqlCommand.Parameters["@id"].Value = "id";
				insertAsrunSqlCommand.Parameters["@title"].Value = titleListBox.Items[indexTitle];
				insertAsrunSqlCommand.Parameters["@s"].Value = "s";
				insertAsrunSqlCommand.Parameters["@status"].Value = "status";
				insertAsrunSqlCommand.Parameters["@ch"].Value = "ch";
				insertAsrunSqlCommand.Parameters["@reconcile"].Value = "reconcile";
				insertAsrunSqlCommand.Parameters["@ty"].Value = "ty";
				mwSqlConnection.Open();
				insertAsrunSqlCommand.ExecuteNonQuery();
				mwSqlConnection.Close();
				statusLabel.Text = String.Format("Application running: Channel {0} Time {1}",
					channels[channelIndex], currentTimecode.ToUniversalTime());
				currentTimecode = currentTimecode.AddSeconds(int.Parse((string) mixListBox.Items[indexDuration]));
			}
			catch (SqlException ex) 
			{
				exceptionLabel.Text = ex.ToString();
				timer1.Stop();
				populateButton.Enabled = true;
			}
		}

		private DateTime getMinDate(int channelIndex) 
		{
			DateTime retval;
			String stmt = "select min(start_tc) from chunk where channel_id = {0} and isblank = 0";
			String fs = String.Format(stmt,channels[channelIndex].ToString());
			SqlCommand cmd = new SqlCommand(fs,mwSqlConnection);
			mwSqlConnection.Open();
			SqlDataReader reader = cmd.ExecuteReader();
			if (reader.Read()) 
			{
				retval = reader.GetDateTime(0);
			} 
			else 
			{
				throw new ArgumentException("No Data for this channel","channelIndex");
			}
			reader.Close();
			mwSqlConnection.Close();
			return(retval);
		}

		private DateTime getMaxDate(int channelIndex)
		{
			DateTime retval;
			String stmt = "select max(start_tc) from chunk where channel_id = {0} and isblank = 0";
			SqlCommand cmd = new SqlCommand(String.Format(stmt,channels[channelIndex].ToString()),mwSqlConnection);
			mwSqlConnection.Open();
			SqlDataReader reader = cmd.ExecuteReader();
			if (reader.Read()) 
			{
				retval = reader.GetDateTime(0);
			} 
			else 
			{
				throw new ArgumentException("No Data for this channel","channelIndex");
			}
			reader.Close();
			mwSqlConnection.Close();
			return(retval);
		}

		private int[] getChannels() 
		{
			ArrayList retval = new ArrayList();
			String stmt = "select distinct(channel_id) from chunk";
			SqlCommand cmd = new SqlCommand(stmt,mwSqlConnection);
			mwSqlConnection.Open();
			SqlDataReader reader = cmd.ExecuteReader();
			while (reader.Read()) 
			{
				retval.Add((int) reader.GetInt32(0));
			} 
			reader.Close();
			mwSqlConnection.Close();
			int[] retarr = new int[retval.Count];
			for (int i=0; i < retval.Count; i++)
			{
				retarr[i] = (int) retval[i];
			}
			return(retarr);
		}

		private void populateButton_Click(object sender, System.EventArgs e)
		{
			channels = getChannels();
			if (channels.Length == 0) 
			{
				return;
			}
			channelIndex = 0;
			currentTimecode = getMinDate(channelIndex);
			thisMax = getMaxDate(channelIndex);
			populateButton.Enabled = false;
			timer1.Start();
		}
	}
}
