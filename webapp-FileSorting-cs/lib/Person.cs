namespace webapp.FileSorting.cs.lib;

internal class Person(string name, string quality, string team)
{
    public string Name { get; set; } = name;
    public string Quality { get; set; } = quality;
    public string Team { get; set; } = team;
}