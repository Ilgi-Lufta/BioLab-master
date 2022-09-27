#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioLab.Models;
public class Mesazh
{
    
    [Key]
    public int MesazhID { get; set; }
    [Required]
    public string Emri { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [Display(Name ="Mesazhi")]
    public string Content { get; set; }
    [Required]
    public int NrTel { get; set; }
    public bool Lexuar { get; set; } = false;

}