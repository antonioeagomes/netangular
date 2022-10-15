namespace Backend.Helpers;

public class UserParams : PaginationParams
{
    private const int MaxPageSize = 50;   

    public string CurrentUsername { get; set; }

    public int MinAge { get; set; } = 18;

    public int MaxAge { get; set; } = 150;

    public string OrderBy { get; set; } = "lastActive";

}