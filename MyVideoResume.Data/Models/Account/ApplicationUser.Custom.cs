namespace MyVideoResume.Data.Models;

public partial class ApplicationUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Picture { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? City { get; set; }
    public string? PostalZipCode { get; set; }
    public string? Country { get; set; }
    public string? Website { get; set; }
    public string? ProfileDescription { get; set; }
}
