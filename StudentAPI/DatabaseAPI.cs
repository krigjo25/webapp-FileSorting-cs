using Microsoft.EntityFrameworkCore;

namespace Webapp.sorting.cs;

public class DatabaseApi : DbContext
{
    public DatabaseApi(DbContextOptions<DatabaseApi> options) : base(options)
    {
    }

    public DbSet<Person> person  => Set<Person>();
}