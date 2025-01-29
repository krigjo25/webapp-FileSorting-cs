namespace Webapp.sorting.cs;

public class Person
{
    public Person(string name, string quality, string team)
    {
        Name = name;
        
        Team = team;
        Quality = quality;
    }
    public string ID { get; set; }
    public string Team { get; set; }
    public string Name { get; set; }
    public string Quality { get; set; }
}  