using Microsoft.EntityFrameworkCore;

namespace WebApplication4;

public class Person
{
    public Person(string name, string quality, string team)
    {
        Name = name;
        Quality = quality;
        Team = team;
    }

    public string Team { get; set; }
    public string Name { get; set; }
    public string Quality { get; set; }
}  

public class DatabaseApi : DbContext
{
    public DatabaseApi(DbContextOptions<DatabaseApi> options) : base(options)
    {
    }

    public DbSet<Person> person { get; set; }
}