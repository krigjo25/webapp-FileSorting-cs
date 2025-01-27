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
    private string InitializeList(List<object> Data)
            {
                //  Initialize the query
                string query = "";
    
                //  Ensure that the data is a list of Person objects
                foreach (var element in Data)
                {
    
                    if (element.GetType() == typeof(Person))
                    {
                        //  Add columns into the query
                        var person = (Person)element;
    
                        query += $"({person.Team}, {person.Name}, {person.Quality.Trim()})";
    
                        //  Ensure that the last element does not have a comma
                        if (person != Data.Last())
                        {
                            query += ", ";
                        }
                    }
                    else
                    {
                        //  Add columns into the query
                        query += element;
    
                        //  Ensure that the last element does not have a comma
                        if (element != Data.Last())
                        {
                            query += ", ";
    
                        }
                        
                    }
                }
                return query;
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
        // Initialize the Table List
        List<string> Tables = [];
        
        //  Ensure that the table does not exist
        string query = $"SELECT TABLE_NAME FROM {_db}.INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = N'{table}'";
        
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
            return;
        }
        
        //  Create a new table
        using (SqlConnection conn = new SqlConnection($"Server={_server};Database=master;User Id={_user};Password={_password};"))
        {
            //   Fetch the databases

            //  Initialize the query
            query = $"CREATE TABLE {table} ({string.Join(", ", columns)})";

            //  Execute the query
            ExecuteQuery(query);
        }
        
    }
    public void CreateDatabase(string db)
        {
            //  Initialize the list
            List<string> Databases = [];
            
            //  Initialize a query to select the data
            string query;
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
                return;
            }
            
            //  Initialize the database
            using (SqlConnection conn = new SqlConnection($"Server={_server};Database=master;User Id={_user};Password={_password};"))
            {
                //   Fetch the databases

                //  Initialize the query
                query = $"CREATE DATABASE {db}";

                //  Execute the query
                ExecuteQuery(query);
            }
        }

    public void InsertData(string table, List<object> columns, List<object> data)
    {
        using (SqlConnection conn = new SqlConnection($"Server={_server};Database=master;User Id={_user};Password={_password};"))
        {
            //   Fetch the databases

            //  Initialize the query
            string query = $"CREATE TABLE {table} ({string.Join(", ", columns)}) VALUES {string.Join(",", data)}";

            //  Execute the query
            //ExecuteQuery(query);
        }
        return;
    }
}