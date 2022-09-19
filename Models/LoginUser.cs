#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace BioLab.Models;
public class LoginUser
{
    // No other fields!
    [Required]
     [MinLength(3)]
    [MaxLength(15)]
    public string Emripacientit { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
    public string Password { get; set; } 
}