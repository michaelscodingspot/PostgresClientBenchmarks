using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

public class SchoolContext : DbContext
{
    private string connectionString = $"Server=localhost;Port=5432;User Id=postgres;Password=p0stgres;Database=testdb;";

	public DbSet<Teacher> Teachers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(connectionString); 
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Teacher>(e => e.ToTable("teachers"));
        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.Property(e => e.Id)
                                .HasColumnName("id")
                                .HasDefaultValueSql("nextval('account.item_id_seq'::regclass)");
            entity.Property(e => e.FirstName).IsRequired().HasColumnName("first_name");
            entity.Property(e => e.LastName).IsRequired().HasColumnName("last_name");
            entity.Property(e => e.Subject).IsRequired().HasColumnName("subject");
            entity.Property(e => e.Salary).HasColumnName("salary");
        });

        base.OnModelCreating(modelBuilder);
    }
}



//[Table(Teacher.TableName)]
public class Teacher : DbContext
{
	static Random _rnd = new Random();

	public const string TableName = "teachers";

    public static Teacher GetRandomTeacher()
    {
        var n = _rnd.Next(1000);
        return new Teacher("N" + n, "L" + n, "S" + n, n);
    }

    public Teacher(int id, string first_name, string last_name, string subject, int salary)
    {
        Id = id;
        FirstName = first_name;
        LastName = last_name;
        Subject = subject;
        Salary = salary;
    }


    public Teacher(string firstName, string lastName, string subject, int salary)
    {
        FirstName = firstName;
        LastName = lastName;
        Subject = subject;
        Salary = salary;
    }

    //[System.ComponentModel.DataAnnotations.Key]
    [Column("id")] 
    public int Id { get; internal set; }

    //[Column("first_name")] 
    public string FirstName { get; internal set; }

    //[Column("last_name")] 
    public string LastName { get; internal set; }

    //[Column("subject")] 
    public string Subject { get; internal set; }

    //[Column("salary")] 
    public int Salary { get; internal set; }
}