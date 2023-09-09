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

            // Retrieve delta data from Oracle
            DataTable deltaData = RetrieveDeltaData(oracleConnection);

            // Create a DataTable to hold the delta data
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Column1", typeof(string)); // Adjust data types as needed
            dataTable.Columns.Add("Column2", typeof(int));
            // Add more columns as needed to match your data schema

            // Transfer the data from the DeltaData DataTable to the DataTable
            foreach (DataRow row in deltaData.Rows)
            {
                dataTable.Rows.Add(row["OracleColumn1"], row["OracleColumn2"]);
                // Add more columns as needed
            }

            // Use SqlBulkCopy to insert the delta data into SQL Server
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConnection))
            {
                bulkCopy.DestinationTableName = "YourSQLServerTable"; // Specify your target table name
                bulkCopy.WriteToServer(dataTable);
            }

            // Optionally, update a last synchronization timestamp or log the sync operation
        }
    }

    static DataTable RetrieveDeltaData(OracleConnection oracleConnection)
    {
        using (OracleCommand oracleCommand = new OracleCommand("SELECT * FROM YourOracleTable WHERE SomeCriteria", oracleConnection))
        using (OracleDataAdapter adapter = new OracleDataAdapter(oracleCommand))
        {
            DataTable deltaData = new DataTable();
            adapter.Fill(deltaData);
            return deltaData;
        }
    }
}
