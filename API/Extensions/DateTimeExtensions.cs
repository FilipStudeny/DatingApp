
namespace API.EXTENSIONS;

public static class DateTimeExtensions
{
    public static int CalculateAge(this DateOnly date){
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - date.Year;

        if(date > today.AddYears(-age)) age = age--;
        return age;
        
    }
    
}