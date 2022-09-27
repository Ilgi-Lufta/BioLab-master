#pragma warning disable CS8618
/* 
Disabled Warning: "Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable."
We can disable this safely because we know the framework will assign non-null values when it constructs this class for us.
*/
using Microsoft.EntityFrameworkCore;

namespace BioLab.Models;
// the MyContext class representing a session with our MySQL database, allowing us to query for or save data
public class MyContext : DbContext 
{ 
    public MyContext(DbContextOptions options) : base(options) { }
    // the "Monsters" table name will come from the DbSet property name
    public DbSet<Analiza> Analizat { get; set; } 
    public DbSet<Pacient> Pacients { get; set; } 

    public DbSet<FleteAnalize> FleteAnalizes { get; set; } 
    
     public DbSet<mtm> mtms { get; set; }
     public DbSet<Mesazh> Mesazhs { get; set; }
     public DbSet<Admin> Admins { get; set; }

     
    



    
}