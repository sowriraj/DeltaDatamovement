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

            // Retrieve the delta data from Oracle (e.g., based on timestamp)
            DataTable deltaData = RetrieveDeltaDataFromOracle(oracleConnection);

            foreach (DataRow row in deltaData.Rows)
            {
                // Identify the unique identifier in your data
                string uniqueIdentifier = row["UniqueIdentifierColumn"].ToString(); // Adjust to your column name

                // Check if the record already exists in SQL Server
                bool recordExists = CheckIfRecordExistsInSqlServer(sqlConnection, uniqueIdentifier);

                if (recordExists)
                {
                    // Record already exists, perform an UPDATE operation
                    UpdateRecordInSqlServer(sqlConnection, row);
                }
                else
                {
                    // Record is new, perform an INSERT operation
                    InsertRecordIntoSqlServer(sqlConnection, row);
                }
            }

            // Optionally, update a last synchronization timestamp or log the sync operation
        }
    }

    static DataTable RetrieveDeltaDataFromOracle(OracleConnection oracleConnection)
    {
        // Retrieve delta data from Oracle, e.g., based on timestamp
        // Implement your query logic here
    }

    static bool CheckIfRecordExistsInSqlServer(SqlConnection sqlConnection, string uniqueIdentifier)
    {
        // Check if the record with the given unique identifier exists in SQL Server
        // Implement your query logic here and return true if the record exists, false otherwise
    }

    static void UpdateRecordInSqlServer(SqlConnection sqlConnection, DataRow row)
    {
        // Perform an UPDATE operation in SQL Server to update the existing record
        // Implement your UPDATE logic here
    }

    static void InsertRecordIntoSqlServer(SqlConnection sqlConnection, DataRow row)
    {
        // Perform an INSERT operation in SQL Server to insert the new record
        // Implement your INSERT logic here
    }
}

///

using (SqlConnection sqlConnection = new SqlConnection(sqlServerConnectionString))
{
    sqlConnection.Open();
    SqlTransaction transaction = sqlConnection.BeginTransaction();

    try
    {
        // Update existing records in SQL Server
        UpdateExistingRecords(sqlConnection, transaction, dataToUpdate);

        // Insert new records into SQL Server
        InsertNewRecords(sqlConnection, transaction, dataToInsert);

        transaction.Commit();
    }
    catch (Exception ex)
    {
        transaction.Rollback();
        // Handle exceptions and log errors
    }
}
