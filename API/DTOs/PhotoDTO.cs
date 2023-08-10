
using System.ComponentModel.DataAnnotations;

namespace API.DTO;


public class PhotoDTO
{
    public int Id { get; set; }
    public int Url { get; set; }
    public bool IsMain { get; set; }

}