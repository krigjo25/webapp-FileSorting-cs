using System.Text;

namespace webapp.FileSorting.cs.lib;

public class FileReader
{
    // https://github.com/Toorq91/GetPreparedTeamsStudents/blob/main/Program.cs
    
    

    //  Initializing the lists
    List<string> Team = [];
    List<string> Student = [];
    List<object> persons = [];
    Dictionary<string, List<object>> columns = [];
    
    SQLConnector SQL = new SQLConnector("", true, "sa","Maximan1", "1434" );
    
    public void ReadFile()
    {
        // Adopted https://github.com/Toorq91/GetPreparedTeamsStudents/blob/main/Program.cs

        //  Initializing the variables
        const string path = "/home/krigjo25/RiderProjects/webapp-FileSorting-cs/webapp-FileSorting-cs/Jeg er uorganisert.txt";

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

        //  Initialize the List of columns
        InitializingList(Team, "Teams");
        InitializingList(Student,"Students");

        // Remove all values
        Team.Clear();
        Student.Clear();
        
        // Read the file
        string name = null;
        string team = null;
        
        //  Read the file
        foreach (var line in File.ReadLines(path))
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
        
        //  Initializing the columns
        string[] arg = ["ID INT PRIMARY KEY AUTOINCREMENT", "Name TEXT NOT NULL"];
        string[] arg2 = ["ID INT UNIQUE() AUTOINCREMENT", "TeamID INT PRIMARY KEY", "Name TEXT NOT NULL", "Quality TEXT NOT NULL"];
        // Send the data to the database
        
        
        InsertDataKeys("Teams", columns);
        InsertDataValues("Students", columns);
        
    }

    public void InitializingList(List<string> column, string[] array, string table)
    {
        //  Add the columns into the list
        for (int i = 0; i < array.Length; i++)
        {
            column.Add(array[i]);
        }
        
        InitializeDatabase(column, table);
    }

    private void InsertDataKeys(string table, Dictionary<string, List<object>> data)
    {
        List<object> query = [];
        List<object> columns = [];
        
        
        if (table == "Teams")
        {
            columns.Add("Name");
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
        SQL.InsertData(table, columns, query);
    }

    private void InitializeDatabase(List<string> column, string table)
    {
        //  Initialize the SQLConnector

        // Create the database
        SQL.CreateDatabase("GetAcademy");

        //  Create the table
        //SQL.CreateTable(table, column);

    }
}
