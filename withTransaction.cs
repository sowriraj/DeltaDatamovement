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

            // Start a SQL Server transaction
            SqlTransaction sqlTransaction = sqlConnection.BeginTransaction();

            try
            {
                // Retrieve and process delta data from Oracle
                ProcessDeltaData(oracleConnection, sqlConnection, sqlTransaction);

                // Commit the transaction if everything is successful
                sqlTransaction.Commit();

                // Optionally, update a last synchronization timestamp or log the sync operation
            }
            catch (Exception ex)
            {
                // Handle exceptions and roll back the transaction in case of an error
                sqlTransaction.Rollback();

                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            finally
            {
                // Clean up resources
                sqlTransaction.Dispose();
            }
        }
    }

    static void ProcessDeltaData(OracleConnection oracleConnection, SqlConnection sqlConnection, SqlTransaction sqlTransaction)
    {
        using (OracleCommand oracleCommand = new OracleCommand("SELECT * FROM YourOracleTable WHERE SomeCriteria", oracleConnection))
        using (OracleDataReader oracleReader = oracleCommand.ExecuteReader())
        using (SqlCommand sqlCommand = new SqlCommand("INSERT INTO YourSQLServerTable (Column1, Column2) VALUES (@Value1, @Value2)", sqlConnection, sqlTransaction))
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
}
