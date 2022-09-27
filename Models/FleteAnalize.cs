#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioLab.Models;
public class FleteAnalize
{
    
    [Key]
    public int FleteAnalizeId { get; set; } 
    [Required]
    public string Emri { get; set; }

    public float Totali { get; set; } = 0;
    public float Paguar { get; set; } = 0;

    public float Zbritja { get; set; } = 0;

     public bool model {get;set;}=false;

    public bool Pagesa { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    [Required]
    public int PacientId  {get;set;} 
    public Pacient? MyPacient {get;set;}

    public int AdminId { get; set; }

    public Admin? MyAdmin { get; set; }
     public List<mtm> mtms  { get; set; } = new List<mtm>();
    //  public List<pacientflete> pacientflete  { get; set; } = new List<pacientflete>();


    
}