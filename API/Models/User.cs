
using System.ComponentModel.DataAnnotations;
using API.EXTENSIONS;

namespace API.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }

    public string Gender { get; set; }
    public string KnownAs  { get; set; }
    public DateOnly DateOfBirth { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;

    public string Introduction { get; set; }
    public string LookingFor { get; set; }
    public string Interests { get; set; }
    public string Country { get; set; }
    public string City { get; set; }

    public string PhotoURL { get; set; }
    public List<Photo> Photos { get; set; } = new();

}