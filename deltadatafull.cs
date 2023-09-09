using System;
using System.Data;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;

class Program
{
    static void Main()
    {
        string oracleConnectionString = "your Oracle connection string here";
        string sqlServerConnectionString = "your SQL Server connection string here";

        using (OracleConnection oracleConnection = new OracleConnection(oracleConnectionString))
        using (SqlConnection sqlConnection = new SqlConnection(sqlServerConnectionString))
        {
            oracleConnection.Open();
            sqlConnection.Open();

            // Perform data migration here.
        }
    }
}


using (OracleCommand oracleCommand = new OracleCommand("SELECT * FROM YourOracleTable", oracleConnection))
using (OracleDataReader oracleReader = oracleCommand.ExecuteReader())
using (SqlCommand sqlCommand = new SqlCommand("INSERT INTO YourSqlServerTable (Column1, Column2, ...) VALUES (@Value1, @Value2, ...)", sqlConnection))
{
    while (oracleReader.Read())
    {
        sqlCommand.Parameters.Clear();
        sqlCommand.Parameters.AddWithValue("@Value1", oracleReader["OracleColumn1"]);
        sqlCommand.Parameters.AddWithValue("@Value2", oracleReader["OracleColumn2"]);
        // Add more parameters as needed.

        sqlCommand.ExecuteNonQuery();
    }
}



using System;
using System.Data;
using System.Data.SqlClient;
using Oracle.ManagedDataAccess.Client;

class Program
{
    static void Main()
    {
        string oracleConnectionString = "your Oracle connection string here";
        string sqlServerConnectionString = "your SQL Server connection string here";

        using (OracleConnection oracleConnection = new OracleConnection(oracleConnectionString))
        using (SqlConnection sqlConnection = new SqlConnection(sqlServerConnectionString))
        {
            oracleConnection.Open();
            sqlConnection.Open();

            // Perform delta data migration here.
        }
    }
}



DateTime lastSyncTime = GetLastSyncTimeFromSqlServer(sqlConnection); // Get the last synchronization time from SQL Server.

using (OracleCommand oracleCommand = new OracleCommand("SELECT * FROM YourOracleTable WHERE ModificationTime > :LastSyncTime", oracleConnection))
{
    oracleCommand.Parameters.AddWithValue(":LastSyncTime", lastSyncTime);

    using (OracleDataReader oracleReader = oracleCommand.ExecuteReader())
    using (SqlCommand sqlCommand = new SqlCommand("INSERT INTO YourSqlServerTable (Column1, Column2, ...) VALUES (@Value1, @Value2, ...)", sqlConnection))
    {
        while (oracleReader.Read())
        {
            sqlCommand.Parameters.Clear();
            sqlCommand.Parameters.AddWithValue("@Value1", oracleReader["OracleColumn1"]);
            sqlCommand.Parameters.AddWithValue("@Value2", oracleReader["OracleColumn2"]);
            // Add more parameters as needed.

            sqlCommand.ExecuteNonQuery();
        }
    }
    
    // Update the last synchronization time in SQL Server to the current time.
    UpdateLastSyncTimeInSqlServer(sqlConnection, DateTime.Now);
}
