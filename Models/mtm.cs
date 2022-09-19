#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BioLab.Models;
public class mtm
{
    
    [Key]
    public int mtmID { get; set; }

    public int AnalizaId { get; set; }
    public int FleteAnalizeId { get; set; }

    public Analiza? Myanaliz { get; set; }

    public FleteAnalize? Myflanaliz { get; set; }
 

    
}
