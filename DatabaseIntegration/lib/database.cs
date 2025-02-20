namespace webapp.FileSorting.cs.lib;

using DotNetEnv;
using System.Linq;
using System.Data.SqlClient;
using Microsoft.Identity.Client;
using System.Collections.Generic;

internal class SQLConnector
{
    // https://learn.microsoft.com/en-us/sql/?view=sql-server-ver16   -   SQL Documentation
    
    private static string? _db;
    private static string? _user;
    private static string? _port;
    private static string? _server;
    private static string? _password;
    
    public SQLConnector()
    {
        
        //  Loading the Environment Variables
        Env.Load();
        
        _db = Environment.GetEnvironmentVariable("MSQL_db");
        _user = Environment.GetEnvironmentVariable("MSQL_user");
        _port = Environment.GetEnvironmentVariable("MSQL_port");
        _server = Environment.GetEnvironmentVariable("MSQL_server");
        _password = Environment.GetEnvironmentVariable("MSQL_passcode");
        
    }

    [Obsolete("Obsolete")]
    private void ExecuteQuery(string query)
    {
        //  Execute the query
        using (var conn = new SqlConnection($"Server={_server};Database={_db};User Id={_user};Password={_password};"))
        {
            var log = new Logger();
            //  Open the connection
            conn.Open();
            
            //  Execute the query
            var cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
            
                //($"{_user} has logged in, and performed the following query: {query}.\n in the server {_server} using database {_db}");
        }
    }

    private bool DuplicationConfirmation(string table = "", string? db = "", List<object> Column = null)
    {
        
        string query;
        var duplicate = false;
        
        if (table != string.Empty)
        {
            // Initialize the Table List
            List<string> Tables = [];

            //  Ensure that the table does not exist
            query = $"SELECT TABLE_NAME FROM {_db}.INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'{table}'";

            //  Select the data
            var data = SelectData(query);

            //  Add the rows into the list
            foreach (var row in data)
            {
                for (int i = 0; i < row.Length; i++)
                {
                    Tables.Add($"{row[i]}");
                }
            }

            //  Ensure that the table does not exist
            if (Tables.Any(element => element.ToString() == table))
            {
                
                duplicate = true;
                Console.WriteLine("Table already exists");
                
            }
            else
            {
                duplicate = false;
            }
        }
        
        
        //  Ensure that the database exists
        if (db != string.Empty)
        {
            //  Initialize the list
            List<string> Databases = [];
            
            //  Initialize a query to select the data
            
            query = "SELECT name FROM sys.databases";

            //   Select the data from the database
            var Data = SelectData(query);
            
            //  Add the rows into the list
            foreach (var row in Data)
            {
                Databases.AddRange(row.Select(t => $"{t}"));
            }
            
            //  Ensure that the database exists
            if (Databases.Any(element => element.ToString() == db)) 
            {
                Console.WriteLine("Database already exists");
                
                //   Assign the database to the current database connection. (Assuming that the user wants to use the existing database)
                _db = db;
                duplicate = true;
            }
            else
            {
                duplicate = false;
            }
        }


        if (Column != null)
        {
            Console.WriteLine("Checking for duplicates");
            //  Initialize the list
            List<string> SqlData = [];
            
            //  Initialize a query to select the data
            
            query = $"SELECT * FROM {table};";
            //   Select the data from the database
            var Data = SelectData(query);
            
            //  Add the rows into the list
            foreach (var row in Data)
            {
                for (int i = 0; i < row.Length; i++)
                {
                    SqlData.Add($"{row[i]}");
                }
            }

            //  Ensure that the database exists
            foreach (var element in Column)
            {
                if (SqlData.Any(el => element.ToString().Contains(el)))
                {
                    Console.WriteLine("One or more rows is duplicated");
                    return true;
                }

                duplicate = false;
            }
        }
        
        return duplicate;
        
        
    }

    private List<object[]> SelectData(string query)
    {
        //  Initialize the list of data
        List<object[]> Data = [];
        
        //  Select the data
        using (var conn = new SqlConnection($"Server={_server};Database={_db};User Id={_user};Password={_password};"))
        {
            conn.Open();
            
            //  Inititalizing a new command
            var cmd = new SqlCommand(query, conn);

            //  Selecting the data
            var reader = cmd.ExecuteReader();
            
            //  Fetch the data
            while (reader.Read())
            {
                //  Initializing a new SQL Data row
                object[] row = new object[reader.FieldCount];

                //  Add the data into the row
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[i] = reader[i];
                }

                Data.Add(row);
            }
            return Data;
        }
    }

    public void InitializeTable(string table, List<string> columns)
    {
        if (DuplicationConfirmation(table) || !DuplicationConfirmation(db:_db))
        {
            return;
        }
        //  Initialize a new table
        string query;
        query = $"CREATE TABLE {table} ({string.Join(", ", columns)})";
        
        //  Execute the query
        ExecuteQuery(query);
        
    }
    
    public void CreateDatabase(string db)
        {
            if (!DuplicationConfirmation("", db))
            {
                return;
            }
            
            //  Initialize the query
            string query = $"CREATE DATABASE {db}";
            
            //  Execute the query
            ExecuteQuery(query);
        }

    public void InsertData(string table, List<object> columns, List<object> data)
    {
        // Ensure the data is not dupblicated and table exists
        if (!DuplicationConfirmation(table) || !DuplicationConfirmation(db:_db) || DuplicationConfirmation(table,Column:data))
        {
            return;
        }
        
        //  Initialize the query
        string query = $"INSERT INTO {table} ({string.Join(", ", columns)}) VALUES {string.Join(",", data)};";
        
        //  Execute the query
        ExecuteQuery(query);

    }
    
    public void DeleteData(string table, string column, string data)
    {
        //  Initialize the query
        string query = $"DELETE FROM {table} WHERE {column} = {data}";
        
        //  Execute the query
        ExecuteQuery(query);

    }
    public void DropTable(string table, string db)
    {
        //  Initialize the query
        string query = $"DROP TABLE {table}";
        
        //  Execute the query
        ExecuteQuery(query);
    }
    public void DropDatabase(string db)
    {
        //  Initialize the query
        string query = $"DROP DATABASE {db}";
        
        //  Execute the query
        ExecuteQuery(query);
    }
}