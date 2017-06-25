using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace MediaWatchDBDetach
{
	/// <summary>
	/// Detach Database from SQL Server.
	/// Delete log files.
	/// </summary>
	class Detach
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			string DBPath = String.Join(" ", args);
			string mdf = System.IO.Path.Combine(DBPath, "MediaWatch.mdf");
			string mdflog = System.IO.Path.Combine(DBPath, "MediaWatch_log.mdf");

			try 
			{
				SqlCommand sqlCmd = new SqlCommand();

				SqlConnection sqlConn = new SqlConnection(
					"data source=(local); initial catalog=master;integrated security=SSPI");
				sqlCmd.Connection = sqlConn;
				sqlConn.Open();

				Console.WriteLine("Checking for and removing existing database...");

				sqlCmd.CommandText =
					"SELECT TOP 1 * FROM [master].[dbo].[sysdatabases] " +
					"WHERE [name]='MediaWatch' " + 
					"IF (@@ROWCOUNT>0) DBCC DETACHDB([MediaWatch])";

				sqlCmd.ExecuteNonQuery();

				sqlConn.Close();
				Console.WriteLine("Database Detach Complete. Press [Return].");
			}
			catch (Exception ex) 
			{
				Console.WriteLine("An error occurred during installation:\r\n" + ex.ToString());
			}
			try
			{
				File.Delete(mdf);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Couldn't delete " + mdf + "\r\n" + ex.ToString());
			}
			try 
			{
				File.Delete(mdflog);
			}
			catch (Exception ex) 
			{
				Console.WriteLine("Couldn't delete " + mdflog + "\r\n" + ex.ToString());
			}
			Console.ReadLine();
		}
	}
}
