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

            // Retrieve and move updated data from Oracle to SQL Server
            MoveUpdatedData(oracleConnection, sqlConnection);

            // You may want to update a last synchronization timestamp or record the sync operation here.
        }
    }

    static void MoveUpdatedData(OracleConnection oracleConnection, SqlConnection sqlConnection)
    {
        using (OracleCommand oracleCommand = new OracleCommand("SELECT * FROM YourOracleTable WHERE ModificationTime > @LastSyncTime", oracleConnection))
        {
            // Set the LastSyncTime parameter to the last synchronization timestamp.
            oracleCommand.Parameters.AddWithValue("@LastSyncTime", GetLastSyncTimeFromSQLServer(sqlConnection));

            using (OracleDataReader oracleReader = oracleCommand.ExecuteReader())
            using (SqlCommand sqlCommand = new SqlCommand("UPDATE YourSQLServerTable SET Column1 = @Value1, Column2 = @Value2 WHERE KeyColumn = @Key", sqlConnection))
            {
                sqlCommand.Parameters.Add("@Value1", SqlDbType.VarChar); // Adjust the SqlDbType as needed.
                sqlCommand.Parameters.Add("@Value2", SqlDbType.VarChar);
                sqlCommand.Parameters.Add("@Key", SqlDbType.Int);

                while (oracleReader.Read())
                {
                    sqlCommand.Parameters["@Value1"].Value = oracleReader["OracleColumn1"];
                    sqlCommand.Parameters["@Value2"].Value = oracleReader["OracleColumn2"];
                    sqlCommand.Parameters["@Key"].Value = oracleReader["PrimaryKeyColumn"]; // Use the appropriate primary key.

                    sqlCommand.ExecuteNonQuery();
                }
            }
        }
    }

    static DateTime GetLastSyncTimeFromSQLServer(SqlConnection sqlConnection)
    {
        // Query the last synchronization time from SQL Server.
        // Implement this based on how you store the last sync time in your SQL Server database.
        // Example: SqlCommand sqlCommand = new SqlCommand("SELECT LastSyncTime FROM SyncInfoTable", sqlConnection);
        // Then execute the command and return the result.
        // DateTime lastSyncTime = (DateTime)sqlCommand.ExecuteScalar();
        // return lastSyncTime;
    }


    import { MatTableDataSource } from '@angular/material/table';
import { YourDataService } from 'path/to/your-data.service';

export class YourComponent {
  dataSource = new MatTableDataSource<any>([]);
  displayedColumns: string[] = ['select', 'column1', 'column2', ...]; // Add other column names here

  constructor(private dataService: YourDataService) {}

  ngOnInit() {
    // Subscribe to the data service to get updates
    this.dataService.getData().subscribe((updatedData: any[]) => {
      // Update only particular data in dataSource
      updatedData.forEach((updatedItem: any) => {
        const index = this.dataSource.data.findIndex((item: any) => item.id === updatedItem.id); // Assuming you have an identifier like 'id'
        if (index !== -1) {
          this.dataSource.data[index] = { ...updatedItem }; // Update the specific item in dataSource.data
        }
      });
      this.dataSource._updateChangeSubscription(); // Trigger data change detection
    });
  }
}
}
