namespace webapp.FileSorting.cs.lib;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;


internal class SQLConnector
{
    private SqlConnection Conn;
    // https://learn.microsoft.com/en-us/sql/?view=sql-server-ver16
    public SQLConnector(string db="", bool trucon = false, string user = "", string password = "", string port = "1143", string server = "localhost\\MSSQLLocalDB")
    {
        string connectionString =$"Server={server},{port};User ID={user};Password={password}";
        
        Conn = new SqlConnection(connectionString);
    }

    
    public void CreateDatabase(string db)
    {

        List<string> Databases = [];
        
        //  Open the connection
        Conn.Open();
        
        //  Initialize the query
        string query = $"CREATE DATABASE GetAcademy";
        
        //  Execute the query
        var cmd = new SqlCommand(query, Conn);
        cmd.ExecuteNonQuery();
        
        //  Close the connection
        Conn.Close();
    }

    public void CreateTable(string table, List <string> columns)
    {
        //  Open the connection
        Conn.Open();
        
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
        
        // Execute the query
        // var cmd = new SqlCommand($"CREATE TABLE IF NOT EXISTS {table}({query})", Conn);
        // cmd.ExecuteNonQuery();
        Console.WriteLine(query);
        
        // Close the connection
       // Conn.Close();
    }

    public void InsertData(string table, List<object> column, List<object> Data = null)
    {
        //  Add columns into the query
        var data = InitializeList(Data);
        foreach (var el in column)
        {
            Console.WriteLine(el);
        }
        var columns = InitializeList(column);
        
        
        //  Initializing the query
        string query = $"INSERT INTO {table} ({columns}) VALUES {data};";
        Console.WriteLine(query);
        
        // //  Open the connection
        // Conn.Open();
        
        //  Execute the query
        //var cmd = new SqlCommand(query, Conn);
        //cmd.ExecuteNonQuery();
        
        //  Close the connection
        //Conn.Close();
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

}