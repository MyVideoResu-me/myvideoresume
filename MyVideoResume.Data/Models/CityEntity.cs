using Microsoft.EntityFrameworkCore;
using MyVideoResume.Abstractions.Core;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyVideoResume.Data.Models;

[Table("Cities")]
public class CityEntity : City
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public StateProvinceEntity StateProvince { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public CountryEntity Country { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public List<PostalZipCodeEntity> PostalZipCodes { get; set; }
}
