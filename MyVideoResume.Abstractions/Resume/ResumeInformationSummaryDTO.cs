namespace MyVideoResume.Abstractions.Resume;

public class ResumeInformationSummaryDTO : ResumeInformationBase, IResumeItem
{
    public required string Id { get; set; }
    public required string TemplateName { get; set; }
    public bool IsPublic { get; set; }
    public bool? IsOwner { get; set; }
    public required string CreationDateTimeFormatted { get; set; }
}
