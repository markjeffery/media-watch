using System;
using System.Data;
using System.Data.SqlClient;

namespace MediaWatchDBInstaller
{
	/// <summary>
	/// Attach to database file, and setup permissions.
	/// </summary>
	class Attach
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			SqlCommand sqlCmd;
			SqlConnection sqlConn;

			try 
			{
				sqlCmd = new SqlCommand();
				string DBPath = String.Join(" ", args);
				DBPath = System.IO.Path.Combine(DBPath, "MediaWatch.mdf");

				sqlConn = new SqlConnection(
					"data source=(local); initial catalog=master;integrated security=SSPI");
				sqlCmd.Connection = sqlConn;
				sqlConn.Open();

				Console.WriteLine("Checking for and removing existing database...");

				sqlCmd.CommandText =
					"SELECT TOP 1 * FROM [master].[dbo].[sysdatabases] " +
					"WHERE [name]='MediaWatch' " + 
					"IF (@@ROWCOUNT>0) DBCC DETACHDB([MediaWatch])";

				sqlCmd.ExecuteNonQuery();

				Console.WriteLine("Installing database from: \"" + DBPath + "...");
				sqlCmd.CommandText = String.Format(
					"CREATE DATABASE [MediaWatch] ON " +
					"(FILENAME='{0}') FOR ATTACH", DBPath);

				sqlCmd.ExecuteNonQuery();

			}
			catch (Exception ex) 
			{
				Console.WriteLine("An error occurred during installation:\r\n" + ex.ToString());
				Console.ReadLine();
				return;
			}

			String userName = String.Format("{0}\\ASPNET",Environment.MachineName);
				
			try
			{
				// TODO: Check for Windows Server 2003, and grant access to 'NT AUTHORITY\NETWORK SERVICE'
				Console.WriteLine("Granting database access to ASP.Net user " + userName);
				sqlCmd.CommandText = "sp_grantlogin";
				sqlCmd.CommandType = CommandType.StoredProcedure;
				sqlCmd.Parameters.Clear();
				sqlCmd.Parameters.Add("@loginame", SqlDbType.NVarChar, 128);
				sqlCmd.Parameters["@loginame"].Value = userName;
				sqlCmd.ExecuteNonQuery();
			}
			catch (Exception ex) 
			{
				Console.WriteLine("An error occurred during installation:\r\n" + ex.ToString());
			}
			try 
			{
				Console.WriteLine("Now logging on to MediaWatch database");
				sqlConn.Close();
				sqlConn = new SqlConnection(
					"data source=(local); initial catalog=MediaWatch;integrated security=SSPI");
				sqlCmd.Connection = sqlConn;
				sqlConn.Open();

				sqlCmd.CommandText = "sp_grantdbaccess";
				sqlCmd.CommandType = CommandType.StoredProcedure;
				sqlCmd.Parameters.Clear();
				sqlCmd.Parameters.Add("@loginame", SqlDbType.NVarChar, 128);
				sqlCmd.Parameters["@loginame"].Value = userName;
				// sqlCmd.Parameters.Add("@name_in_db", SqlDbType.NVarChar, 128);
				// sqlCmd.Parameters["@name_in_db"].Direction = ParameterDirection.Output;
				sqlCmd.ExecuteNonQuery();
			}
			catch (Exception ex) 
			{
				Console.WriteLine("An error occurred during installation:\r\n" + ex.ToString());
			}
			try
			{
				Console.WriteLine("Granting database access to ASP.Net user");
				sqlCmd.CommandText = "sp_addrolemember";
				sqlCmd.CommandType = CommandType.StoredProcedure;
				sqlCmd.Parameters.Clear();
				sqlCmd.Parameters.Add("@rolename", SqlDbType.NVarChar, 128);
				sqlCmd.Parameters["@rolename"].Value = "db_owner";
				sqlCmd.Parameters.Add("@membername", SqlDbType.NVarChar, 128);
				sqlCmd.Parameters["@membername"].Value = userName;
				sqlCmd.ExecuteNonQuery();

				sqlConn.Close();
				Console.WriteLine("Database Installation Complete. Press [Return].");
			}
			catch (Exception ex) 
			{
				Console.WriteLine("An error occurred during installation:\r\n" + ex.ToString());
			}
			Console.ReadLine();
		}
	}
}
