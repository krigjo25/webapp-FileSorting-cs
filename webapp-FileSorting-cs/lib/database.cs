using System.ComponentModel.DataAnnotations.Schema;

namespace webapp.FileSorting.cs.lib;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;

internal class SQLConnector
{
    // https://learn.microsoft.com/en-us/sql/?view=sql-server-ver16   -   SQL Documentation
    
    private string _db;
    private readonly string _user;
    private readonly string _port;
    private readonly string _server;
    private readonly string _password;
    
    public SQLConnector(string db="", bool trucon = false, string user = "", string password = "", string port = "1143", string server = "localhost")
    {
        _db = db;
        _user = user;
        _port = port;
        _server = server;
        _password = password;
        
    }

    private void ExecuteQuery(string query)
    {
        //  Execute the query
        using (var conn = new SqlConnection($"Server={_server};Database={_db};User Id={_user};Password={_password};"))
        {
            //  Open the connection
            conn.Open();
            
            //  Execute the query
            var cmd = new SqlCommand(query, conn);
            cmd.ExecuteNonQuery();
        }
    }

    private bool DuplicationConfirmation(string table = "", string db = "", List<object> Column = null)
    {
        
        string query;
        bool exists = false;
        
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
                Console.WriteLine("Table already exists");
                exists = true;
                
            }
        }
        else
        {
            exists = false;
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
                for (int i = 0; i < row.Length; i++)
                {
                    Databases.Add($"{row[i]}");
                }
            }
            
            //  Ensure that the database exists
            if (Databases.Any(element => element.ToString() == db)) 
            {
                Console.WriteLine("Database already exists");
                
                //   Assign the database to the current database connection. (Assuming that the user wants to use the existing database)
                _db = db;
                exists = true;
            }
        }
        else
        {
            exists = false;
        }
        
        //  Ensure that the data is not duplicated
        if (Column != null)
        {
            
            //  Initialize the list
            List<string> SqlData = [];
            
            //  Initialize a query to select the data
            
            query = $"SELECT * FROM {table};";
            Console.WriteLine(query);
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
                foreach (var el in SqlData)
                {
                    if (el == element.ToString())
                    {
                        Console.WriteLine("One or more rows is duplicated");
                        exists = true;
                    }
                }
            }
        }
        else
        {
            exists = false;
        }
        
        return exists;
        
        
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
        if (DuplicationConfirmation(table, _db))
        {
            return;
        }
        string query;
        
        //  Initialize a new table
        query = $"CREATE TABLE {table} ({string.Join(", ", columns)})";
        
        using (SqlConnection conn = new SqlConnection($"Server={_server};Database=master;User Id={_user};Password={_password};"))
        {
            //  Execute the query
            ExecuteQuery(query);
        }
        
    }
    public void CreateDatabase(string db)
        {
            if (!DuplicationConfirmation(db))
            {
                return;
            }
            //  Initialize the query
            string query = $"CREATE DATABASE {db}";
            
            //  Initialize the database
            using (SqlConnection conn = new SqlConnection($"Server={_server};Database=master;User Id={_user};Password={_password};"))
            {
                //  Execute the query
                ExecuteQuery(query);
            }
        }

    public void InsertData(string table, List<object> columns, List<object> data)
    {
        // Ensure the data is not dupblicated and table exists
        if (!DuplicationConfirmation(table) || !DuplicationConfirmation("", _db) && DuplicationConfirmation(table, "", data))
        {
            return;
        }
        
        //  Initialize the query
        string query = $"INSERT INTO {table} ({string.Join(", ", columns)}) VALUES {string.Join(", ", data)}";
        
        //  Insert the data
        using (SqlConnection conn = new SqlConnection($"Server={_server};Database=master;User Id={_user};Password={_password};"))
        {
            //  Execute the query
            ExecuteQuery(query);

        }
    }
}