using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MyVideoResume.Abstractions.MetaContent;

namespace MyVideoResume.Data.Models.MetaContent;

[Table("MetaData")]
public class MetaDataEntity : MetaData
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
}
