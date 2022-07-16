using DatingAppAPI.Extensions;

namespace DatingAppAPI.DTOs;

public class MemberDto
{
    public int Id { get; set; }
    [JsonPropertyName("username")]
    public string UserName { get; set; }

    public string PhotoUrl { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string KnownAs { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastActivate { get; set; }
    public string Gender { get; set; }
    public string Introduction { get; set; }
    public string LookingFor { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public ICollection<PhotoDto> Photos { get; set; }
    public int Age
    {
        get => DateOfBirth.CalculateAge();
        private set { }
    }
}