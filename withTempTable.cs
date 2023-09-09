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

            // Create a temporary table in SQL Server to stage the delta data
            CreateTempTable(sqlConnection);

            // Retrieve and stage delta data from Oracle to the SQL Server temporary table
            StageDeltaData(oracleConnection, sqlConnection);

            // Merge or process data in the temporary table as needed
            // For simplicity, let's just print the data in this example
            ProcessDeltaData(sqlConnection);

            // Optionally, drop the temporary table in SQL Server
            DropTempTable(sqlConnection);
        }
    }

    static void CreateTempTable(SqlConnection sqlConnection)
    {
        using (SqlCommand sqlCommand = new SqlCommand(@"
            CREATE TABLE #TempDeltaData (
                Column1 VARCHAR(255),
                Column2 INT,
                -- Define columns based on your data schema
            );", sqlConnection))
        {
            sqlCommand.ExecuteNonQuery();
        }
    }

    static void StageDeltaData(OracleConnection oracleConnection, SqlConnection sqlConnection)
    {
        using (OracleCommand oracleCommand = new OracleCommand("SELECT * FROM YourOracleTable WHERE SomeCriteria", oracleConnection))
        using (OracleDataReader oracleReader = oracleCommand.ExecuteReader())
        using (SqlCommand sqlCommand = new SqlCommand("INSERT INTO #TempDeltaData (Column1, Column2) VALUES (@Value1, @Value2)", sqlConnection))
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
    }

    static void ProcessDeltaData(SqlConnection sqlConnection)
    {
        // Process or merge data in the temporary table as needed
        using (SqlCommand sqlCommand = new SqlCommand("SELECT * FROM #TempDeltaData", sqlConnection))
        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
        {
            while (sqlDataReader.Read())
            {
                Console.WriteLine($"Column1: {sqlDataReader["Column1"]}, Column2: {sqlDataReader["Column2"]}");
                // Add more processing logic as needed
            }
        }
    }

    static void DropTempTable(SqlConnection sqlConnection)
    {
        using (SqlCommand sqlCommand = new SqlCommand("DROP TABLE #TempDeltaData", sqlConnection))
        {
            sqlCommand.ExecuteNonQuery();
        }
    }
}
