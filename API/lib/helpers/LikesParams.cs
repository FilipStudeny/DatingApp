namespace API.LIB.HELPERS;

public class LikesParams : PaginationParams
{
    public int UserID { get; set; }
    public string predicate { get; set; }
}

