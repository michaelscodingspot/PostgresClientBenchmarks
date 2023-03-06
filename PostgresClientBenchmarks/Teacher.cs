using System;
using System.ComponentModel.DataAnnotations.Schema;
using BenchmarkDotNet.Toolchains.CoreRun;
using Microsoft.EntityFrameworkCore;



public class SchoolContext : DbContext
{
    public DbSet<Teacher> Teachers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Postgres.ConnectionString); 
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
    public const string TableName = "teachers";

    public static Teacher GetRandomTeacher()
    {
        Random rnd = new Random();
        var n = rnd.Next(1000);
        return new Teacher("N" + n, "L" + n, "S" + n, n);
    }

    public Teacher(int id, string first_name, string last_name, string subject, int salary)
    {
        throw new NotImplementedException();
        //Id = id;
        //FirstName = first_name;
        //LastName = last_name;
        //Subject = subject;
        //Salary = salary;
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