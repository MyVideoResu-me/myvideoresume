namespace MyVideoResume.Abstractions.Resume;

public class ResumeInformationSummaryDTO : ResumeInformationBase, IResumeItem
{
    public string Id { get; set; }
    public string TemplateName { get; set; }
    public bool IsPublic { get; set; }
    public bool? IsOwner { get; set; }
    public string CreationDateTimeFormatted { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
}
