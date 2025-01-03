using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities;

[Table("Photos")]
public class Photo
{
    public int Id { get; set; }
    public required string Url { get; set; }
    public bool IsMain { get; set; }
    public bool IsApproved { get; set; } = false;
    public string? PublicId { get; set; }

    //navigation properties
    //make the userId is required (not nullable) all photos should be related to user
    public int AppUserId { get; set; }
    public AppUser AppUser { get; set; } = null!;
}