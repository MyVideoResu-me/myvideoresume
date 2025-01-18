using Microsoft.EntityFrameworkCore;
using MyVideoResume.Abstractions.Core;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyVideoResume.Data.Models;

[Table("PostalZipCodes")]
public class PostalZipCodeEntity : PostalZipCodeItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public CountryEntity Country { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public List<CityEntity> Cities { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public List<StateProvinceEntity> StateProvinces { get; set; }

}
