using DatingAppAPI.Extensions;

namespace DatingAppAPI.Entities;

public class AppUser
{
    public int Id { get; set; }
    public string UserName { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string KnownAs { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;
    public DateTime LastActive { get; set; } = DateTime.Now;
    public string Gender { get; set; }
    public string Introduction { get; set; }
    public string LookingFor { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public ICollection<Photo> Photos { get; set; }
    public AppUser() { }
    public int Age
    {
        get => DateOfBirth.CalculateAge();
        private set { }
    }
    public void UpdatePassword(string password)
    {
        using var hmac = new HMACSHA512();
        PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        PasswordSalt = hmac.Key;
    }
    public void UpdateDateOfBirth(DateTime dateOfBirth)
    {
        DateOfBirth = dateOfBirth;
    }
    public void UpdateKnownAs(string knownAs)
    {
        KnownAs = knownAs;
    }
    public void UpdateLastActive(DateTime lastActive)
    {
        LastActive = lastActive;
    }
    public void UpdateGender(string gender)
    {
        Gender = gender;
    }
    public void UpdateIntroduction(string introduction)
    {
        Introduction = introduction;
    }
    public void UpdateLookingFor(string lookingFor)
    {
        LookingFor = lookingFor;
    }
    public void UpdateCity(string city)
    {
        City = city;
    }
    public void UpdateCountry(string country)
    {
        Country = country;
    }
}

