
using Microsoft.EntityFrameworkCore;

public class Workplace : DbContext
{
    public DbSet<Worker> Workers { get; set; }
    public DbSet<Request> VacationRequests { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder dbcob)
    {
        dbcob.UseSqlServer(@"data Source=.\SQLEXPRESS;initial catalog=RequestDb; integrated security=SSPI");
        base.OnConfiguring(dbcob);
    }
}