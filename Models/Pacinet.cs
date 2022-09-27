#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioLab.Models;
public class Pacient
{
    
    [Key]
    public int PacientId { get; set; } 
 
    [Required]
    [Display(Name = "Emri Pacientit")]
    public string Emripacientit { get; set; }
    [Required]
    [Display(Name = "Numri Personal")]
    public string NrPersonal { get; set; }

    public string Gjinia{ get; set; }

    public string Tipi{ get; set; }

     public int AdminId { get; set; }
     public Admin? MyAdmin { get; set; }

    public int Mosha{ get; set; } 

     public string Password { get; set; }= string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
     public List<FleteAnalize> MYfleteanaliz  { get; set; } = new List<FleteAnalize>();
    


    
}