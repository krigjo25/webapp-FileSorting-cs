using DotNetEnv;

namespace webapp.FileSorting.cs.lib;

public class FileReader
{
    
    //  Initializing the lists
    List<string> Team = [];
    List<string> Student = [];
    List<object> persons = [];
    
    // Dictionary to store the columns
    Dictionary<string, List<object>> columns = [];

    private SQLConnector SQL = new SQLConnector();
    
    public void ReadFile(string path)
    {
        /*
         *  Adopted from https://github.com/Toorq91/GetPreparedTeamsStudents/blob/main/Program.cs
         *  The code reads a file and stores the data in a database 
         */
        
        //  Loading the Environment Variables
        Env.Load();
        
        
        // Create the database
        SQL.CreateDatabase($"{Environment.GetEnvironmentVariable("MSQL_DB")}");
        
        try
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException("File was not found in the seleceted path");
            }
        }
        catch (FileNotFoundException e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        InitializeTables();
        
        //  Initialize the variables
        string name = null;
        string team = null;
        
        //  Read the file
        using (var f = File.OpenText(path))
        {
            while (f.ReadLine() is { } line)
            {
                line = line.Trim();
                //  Ensure that the line contains the word "Team"
                if (line.Contains("Team"))
                {
                    team = line;
                }
                
                //  Ensure that the line contains a name
                else if (name == null)
                {
                    name = line;
                }
            
                //  Add the person to the list
                else
                {
                    var person = new Person(name, line, team);
                    persons.Add(person);
                
                    columns[team] = persons;
                
                    name = null;
                }
            }
        }
        
        // Send the data to the database
        InsertValuesToTeams("Teams", columns);
        InsertValuesToStudents("Students", columns);
        
    }
    
    private void InitializeTables()
    {
        //  Initializing the columns
        string[] arg = ["ID INT IDENTITY(1,1) PRIMARY KEY", "Name TEXT NOT NULL"];
        string[] arg1 = ["ID INT IDENTITY(1,1) PRIMARY KEY", "TeamID TEXT NOT NULL", "Name TEXT NOT NULL", "Quality TEXT NOT NULL"];
        
        //  Initialize the List of columns
        InitializingList(arg, "Teams");
        InitializingList(arg1,"Students");
    }
    
    private void InitializingList(string[] array, string table)
    {
        List<string> column = [];
        //  Add the columns into the list
        for (int i = 0; i < array.Length; i++)
        {
            column.Add(array[i]);
        }
        
        //  Create the table
        SQL.InitializeTable(table, column);
        
    }

    private void InsertValuesToTeams(string table, Dictionary<string, List<object>> data)
    {
        List<object> query = [];
        List<object> columns = [];
        
        columns.Add("Name");

        
        foreach (var element in data.Keys)
        {
            query.Add($"('{element}')");

        }
        SQL.InsertData(table, columns, query);
        
        
    }
    private void InsertValuesToStudents(string table, Dictionary<string, List<object>> data)
    {
        //  Initialize the list of queries and columns
        List<object> columns = [];
        columns.AddRange("TeamID", "Name", "Quality");
        
        List<object> query = [];
        foreach (var obj in data.Values.SelectMany(element => element))
        {
            //  Ensure that the data is of type Person
            if (obj is Person person)
            {
                // Initialize the object into a string
                string str = $"('{person.Team}', '{person.Name}', '{person.Quality}')";
                query.Add(str);
            }
        }
        
        //  Initialize the SQLConnector
        SQL.InsertData(table, columns, query);
    }
    
}
