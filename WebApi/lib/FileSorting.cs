namespace WebApi.lib;

public class Person
{
    /*
     * This is a class that represents a person API
     */
    //	Properties
    public string Name;
    public string Quality;
    public Dictionary<string,string> Team;

    // Constructor
    public Person(string name, string quality, Dictionary<string, string> team)
    {
        Name = name;
        Team = team;
        Quality = quality;
    }
}