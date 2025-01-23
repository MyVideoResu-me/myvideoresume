using MyVideoResume.Abstractions.Core;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace MyVideoResume.Data.Models;

[Table("StateProvinces")]
public class StateProvinceEntity : StateProvince
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public CountryEntity Country { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public List<CityEntity> Cities { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public List<PostalZipCodeEntity> PostalZipCodes { get; set; }

}
