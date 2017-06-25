using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Collections;

namespace HotFolderMonitorInteractive
{
	/// <summary>
	/// Holds configuration information, and keeps it updated given changes
	/// Uses backup of main configuration file:
	/// c:\Program Files\Sony\MediaWatchServer\Server\ServerConfig.xml
	/// into ServerConfigBackup.xml in local directory.
	/// </summary>
	public class Config
	{
		private SqlConnection mwConnection;

		private FileSystemWatcher configWatcher;
		public Config()
		{
			try 
			{
				System.Configuration.AppSettingsReader configurationAppSettings = new System.Configuration.AppSettingsReader();
				this.mwConnection = new System.Data.SqlClient.SqlConnection();
				this.mwConnection.ConnectionString = ((string)(configurationAppSettings.GetValue("mwConnection.ConnectionString", typeof(string))));

				configWatcher = new FileSystemWatcher();
				configWatcher.EnableRaisingEvents = false;
				configWatcher.Filter = ((string)(configurationAppSettings.GetValue("ServerConfig.Filter", typeof(string))));
				configWatcher.Path = ((string)(configurationAppSettings.GetValue("ServerConfig.Path", typeof(string))));
				configWatcher.Created += new FileSystemEventHandler(newConfigFile);
				configWatcher.Changed += new FileSystemEventHandler(updatedConfigFile);

				// load config file
				string fullname = ((string)(configurationAppSettings.GetValue("ServerConfig.Path", typeof(string)))) + 
					Path.DirectorySeparatorChar + ((string)(configurationAppSettings.GetValue("ServerConfig.Filter", typeof(string)))); 
				loadConfigFile(fullname);
			}
			catch (Exception ex) 
			{
				Console.WriteLine("Exception " + ex.ToString());
			}
		}

		/// <summary>
		/// Start looking for changes to config file.
		/// </summary>
		public void startWatching() 
		{
			configWatcher.EnableRaisingEvents = true;
		}

		/// <summary>
		/// Stop looking for changes to config file.
		/// </summary>
		public void stopWatching()
		{
			configWatcher.EnableRaisingEvents = false;
		}

		/// <summary>
		/// Called when config file is created.
		/// As this is a new config file, clear all channels from database.
		/// Then call loadConfigFile to get new channels.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void newConfigFile(object sender, FileSystemEventArgs e)
		{
			// new config file has been created.
			// Execute stored procedure dbo.ClearAll
			// for each channel, insert new channel into database.
			try 
			{
				mwConnection.Open();
				SqlCommand sc = new SqlCommand("dbo.ClearAll",mwConnection);
				sc.CommandType = CommandType.StoredProcedure;
				int nrows = sc.ExecuteNonQuery();
				mwConnection.Close();
				loadConfigFile(e.FullPath);
			}
			catch (Exception ex) 
			{
				Console.WriteLine("Exception " + ex.ToString());
			}
		}

		/// <summary>
		/// LoadConfigFile.
		/// Called when config file changed or created, but also called when folder monitor
		/// is started up. This will provide information on active channels, and allow folder
		/// monitor to know what to start watching for.
		/// TODO: Implement removal of data if channel removed from config file.
		/// </summary>
		/// <param name="fullPath">Full Pathname for Server Config File</param>
		private void loadConfigFile(String fullPath) {
			// todo: implement removal of data if removed from config file.
			// how it would work:
			// create list of current channels by reading into a dataset.
			// subtract list of channels in config file.
			// delete from channel, asrun and chunk rows for remaining channels.
			try
			{
				DataSet ds = new DataSet("channel");
				ds.ReadXml(fullPath);
				mwConnection.Open();
				SqlCommand sc = new SqlCommand("dbo.InsertChannel",mwConnection);
				sc.CommandType = CommandType.StoredProcedure;
				sc.Parameters.Add("RetVal", SqlDbType.Int);
				sc.Parameters["Retval"].Direction = ParameterDirection.ReturnValue;
				sc.Parameters.Add("@fps",SqlDbType.Int, 4);
				sc.Parameters["@fps"].Direction = ParameterDirection.Input;
				sc.Parameters.Add("@id",SqlDbType.Int, 4);
				sc.Parameters["@id"].Direction = ParameterDirection.Input;
				sc.Parameters.Add("@name",SqlDbType.NVarChar, 50);
				sc.Parameters["@name"].Direction = ParameterDirection.Input;
				sc.Parameters.Add("@hotfolder_chunk",SqlDbType.NVarChar, 512);
				sc.Parameters["@hotfolder_chunk"].Direction = ParameterDirection.Input;
				sc.Parameters.Add("@hotfolder_asrun",SqlDbType.NVarChar, 512);
				sc.Parameters["@hotfolder_asrun"].Direction = ParameterDirection.Input;
				foreach (DataRow recorderRow in ds.Tables["MediaWatchRecorder"].Rows)
				{
					sc.Parameters["@fps"].Value = recorderRow["lineStandard"].Equals("625") ? 25 : 30;
					foreach (DataRow channelRow in recorderRow.GetChildRows(ds.Tables["MediaWatchRecorder"].ChildRelations["MediaWatchRecorder_channel"])) 
					{
						sc.Parameters["@id"].Value = channelRow["inputID"].ToString();
						sc.Parameters["@name"].Value = channelRow["userID"].ToString();
						sc.Parameters["@hotfolder_chunk"].Value = channelRow["mediaOutputHotFolder"];
						sc.Parameters["@hotfolder_asrun"].Value = "c:\\Program Files\\Sony\\MediaWatchServer\\asrunhotfolder";
						sc.ExecuteNonQuery();
					}
				}
				mwConnection.Close();
			}
			catch (Exception ex) 
			{
				Console.WriteLine(ex.ToString());
			}
		}

		/// <summary>
		/// Called when Server config file changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void updatedConfigFile(object sender, FileSystemEventArgs e)
		{
			loadConfigFile(e.FullPath);
		}
	}
}
