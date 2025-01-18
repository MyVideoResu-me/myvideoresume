using MyVideoResume.Abstractions.Core;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace MyVideoResume.Data.Models;

[Table("Countries")]
public class CountryEntity : Country
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public List<CityEntity> Cities { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public List<StateProvinceEntity> StateProvinces { get; set; }
}
