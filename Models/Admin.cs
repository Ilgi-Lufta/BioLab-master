#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace BioLab.Models;
public class Admin
{
    [Key]
    public int AdminId { get; set; } 

    [Required]
    
    public string Username { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
    public string Password { get; set; } 

    public List<Analiza> MyAnaliz { get; set; } = new List<Analiza>();
    public List<FleteAnalize> MyfletAnaliz { get; set; } = new List<FleteAnalize>();

    public List<Pacient> MyPacient { get; set; } = new List<Pacient>();



}