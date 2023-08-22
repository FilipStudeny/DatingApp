namespace API.LIB.HELPERS;

public class UserParams : PaginationParams
{

    public string CurrentUsername { get; set; }
    public string Gender { get; set; }
    public int MinAge { get; set; } = 18;//min-age
    public int MaxAge { get; set; } = 100; //max-age

    public string OrderBy { get; set; } = "lastActive";
}

