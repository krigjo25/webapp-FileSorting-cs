using System.Text;

namespace webapp.FileSorting.cs.lib;

public class FileReader
{
    // https://github.com/Toorq91/GetPreparedTeamsStudents/blob/main/Program.cs
    
    //  Initializing the lists
    List<string> Team = [];
    List<string> Student = [];
    List<object> persons = [];
    
    // Dictionary to store the columns
    Dictionary<string, List<object>> columns = [];
    
    SQLConnector SQL = new SQLConnector("", true, "sa","Maximan1", "1434" );

   
    public void ReadFile(string path)
    {
        // https://github.com/Toorq91/GetPreparedTeamsStudents/blob/main/Program.cs
        
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
                //  Ensure that the line contains the word "Team"
                if (line.Contains("Team"))
                {
                    team = line;
                }
                
                //  Ensure that the line contains a name
                else if (name == null)
                {
                    name = line.Trim();
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
        //InsertDataValues("Students", columns);
        
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
    
    public void InitializingList(string[] array, string table)
    {
        List<string> column = [];
        //  Add the columns into the list
        for (int i = 0; i < array.Length; i++)
        {
            column.Add(array[i]);
        }
        
        InitializeDatabase(column, table);
    }

    private void InsertValuesToTab(string table, Dictionary<string, List<object>> data)
    {
        List<object> query = [];
        List<object> columns = [];
        
        switch (table)
        {
            case "Teams":
                columns.Add("Name");
                break;
            case "Students":
                columns.AddRange("TeamID", "Name", "Quality");
                break;
            default:
                return;
        }
        
        foreach (var element in data.Keys)
        {
            query.Add(element);
            
            SQL.InsertData(table, columns, query);
            
            
            query.Remove(element);

        }
        
        
    }
    private void InsertDataValues(string table, Dictionary<string, List<object>> data)
    {
        List<object> columns = [];
        List<object> query = [];
        
        
        columns.AddRange("TeamID", "Name", "Quality");
        query.AddRange(data.SelectMany(element => element.Value));

        //  Initialize the SQLConnector
        //SQL.InsertData(table, columns, query);
    }

    private void InitializeDatabase(List<string> column, string table)
    {
        //  Initialize the SQLConnector

        // Create the database
        SQL.CreateDatabase("GetAcademy");

        //  Create the table
        SQL.InitializeTable(table, column);

    }
}
