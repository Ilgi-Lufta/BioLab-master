#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioLab.Models;
public class Analiza
{
    
    [Key]
    public int AnalizaId { get; set; } 
    [Required]
    public string Emri { get; set; }

    public float Rezultati { get; set; }
    
    [Required]
    public string Njesia{ get; set; }
    [Required]
    public string Norma{ get; set; }

     [Required]
    public float Cmimi{ get; set; }


    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;

     public List<mtm> mtms  { get; set; } = new List<mtm>();

      public int AdminId { get; set; }

    public Admin? MyAdmin { get; set; }


 
 

    
}