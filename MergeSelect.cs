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

            // Retrieve delta data using a MERGE statement with SELECT
            DataTable deltaData = GetDeltaDataUsingMerge(oracleConnection, sqlConnection);

            // Use SqlBulkCopy to insert the delta data into SQL Server
            using (SqlBulkCopy bulkCopy = new SqlBulkCopy(sqlConnection))
            {
                bulkCopy.DestinationTableName = "YourSQLServerTable"; // Specify your target table name
                bulkCopy.WriteToServer(deltaData);
            }

            // Optionally, update a last synchronization timestamp or log the sync operation
        }
    }

    static DataTable GetDeltaDataUsingMerge(OracleConnection oracleConnection, SqlConnection sqlConnection)
    {
        // Create a DataTable to store the delta data
        DataTable deltaData = new DataTable();
        deltaData.Columns.Add("Column1", typeof(string)); // Adjust data types as needed
        deltaData.Columns.Add("Column2", typeof(int));
        // Add more columns as needed to match your data schema

        // Define the MERGE statement with the OUTPUT clause
        string mergeQuery = @"
            MERGE INTO YourSQLServerTable AS Target
            USING (
                SELECT * FROM YourOracleTable WHERE SomeCriteria
            ) AS Source
            ON Target.PrimaryKeyColumn = Source.PrimaryKeyColumn
            WHEN MATCHED THEN
                -- Optionally, update the target table
            WHEN NOT MATCHED BY TARGET THEN
                INSERT (Column1, Column2)
                VALUES (Source.OracleColumn1, Source.OracleColumn2)
            OUTPUT INSERTED.Column1, INSERTED.Column2;"; // Include all columns needed

        using (SqlCommand sqlCommand = new SqlCommand(mergeQuery, sqlConnection))
        using (SqlDataReader sqlDataReader = sqlCommand.ExecuteReader())
        {
            while (sqlDataReader.Read())
            {
                DataRow newRow = deltaData.NewRow();
                newRow["Column1"] = sqlDataReader["Column1"];
                newRow["Column2"] = sqlDataReader["Column2"];
                // Add more columns as needed
                deltaData.Rows.Add(newRow);
            }
        }

        return deltaData;
    }
}
