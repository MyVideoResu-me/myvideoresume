using System.Collections.Generic;

namespace MyVideoResume.Abstractions.Resume;

public class ResumeSearchRequestDTO
{
    public string? TextQuery { get; set; }
    public List<string>? Skills { get; set; }
    public string? Education { get; set; }
    public string? Experience { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? RadiusMiles { get; set; }
    public int? Take { get; set; }
    public int? Skip { get; set; }
}
