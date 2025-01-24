namespace webapp.FileSorting.cs.lib;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;

internal class SQLConnector
{
    // https://learn.microsoft.com/en-us/sql/?view=sql-server-ver16   -   SQL Documentation
    
    private readonly string _db;
    
    private readonly SqlConnection Conn;
    
    
    public SQLConnector(string db="", bool trucon = false, string user = "", string password = "", string port = "1143", string server = "localhost\\MSSQLLocalDB")
    {
        
        _db = db;

        //  Initialize the connection string
        string connectionString = $"Server={server},{port};Database={db};User ID={user};Password={password}";
        
        //  Initialize the connection
        Conn = new SqlConnection(connectionString);

        try
        {
            if (Conn == null)
            {
                throw new Exception("Connection failed");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }

        Console.WriteLine("Connection successful");

    }

    
    public void CreateDatabase(string db)
    {

        //  Initialize the list
        List<string> Databases = [];

        string query;
        
        // Ensure that the database exists
        using (Conn)
        {
            Conn.Open();
            
            //   Fetch the databases
            query = "SELECT name FROM sys.databases";
            
            var Data = SelectData(query);
            
            using (Data)
            {
                while (Data.Read())
                {
                    var element = Data.GetString(0);
                    Databases.Add(element);
                }
            }
        }

        if (Databases.Any(element => element.ToString() == db))
        {
            Console.WriteLine("Database already exists");
            return;
        }
        
        
        using (Conn)
        {
            //   Fetch the databases
            
            //  Initialize the query
            query = $"CREATE DATABASE {db}";
        
            //  Execute the query
            var cmd = new SqlCommand(query, Conn);
            //cmd.ExecuteNonQuery();
            Console.WriteLine(query);
        }

    }

    public void CreateTable(string table, List <string> columns)
    {
        //  Initializing Tables
        List<string> Tables = [];

        using (var con = Conn)
        {
            //  Fetch the tables
            
            //  Add to list
            
        }

        foreach (var element in Tables)
        {
            if (element == table)
            {
                Console.WriteLine("Table already exists");
                return;
            }
        }
        
        //  Initialize the query
        string query = "";
        
        //  Add columns into the query
        foreach (var element in columns)
        {
            //  Add the element into the query
            query += element;
            
            //  Ensure that the last element does not have a comma
            if (element != columns.Last()  )
            {
                query += ", ";
                
            }
            else
            {
                query += ");";
            }
        }
        using (Conn)
        {
            
        
            // Execute the query
            //var cmd = new SqlCommand($"CREATE TABLE IF NOT EXISTS {table}({query})", Conn);
            //cmd.ExecuteNonQuery();
        }
        Console.WriteLine(query);

    }

    public void InsertData(string table, List<object> column, List<object> Data = null)
    {
        
        //  Ensure that the table exists
        List<string> Tables = [];
        
        using (var con = Conn)
        {
            //  Fetch the tables
            
            //  Add to list
            
        }
        
        //   Ensure that the table exists
        foreach (var element in Tables)
        {
            if (element != table )
            {
                if (element == Tables.Last())
                {
                    Console.WriteLine("Table does not exist");
                    return;
                }
            }
        }
        
        //  Add columns into the query
        var data = InitializeList(Data);
        var columns = InitializeList(column);
        
        //  Initializing the query
        string query = $"INSERT INTO {table} ({columns}) VALUES {data};";
        Console.WriteLine(query);
        
        // //  Open the connection

        using (Conn)
        {
            //  Execute the query
            var cmd = new SqlCommand(query, Conn);
            //cmd.ExecuteNonQuery();
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

    private SqlDataReader? SelectData(string query)
    {
        using (Conn)
        {
            
            //  Execute the query
            var cmd = new SqlCommand(query, Conn);
            var reader = cmd.ExecuteReader();

            return reader.HasRows ? reader : null;
        }
    }
    
}