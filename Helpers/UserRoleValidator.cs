namespace Ergasia_WebApp.Helpers;

public static class UserRoleValidator
{
    public static bool Validate(string role)
    {
        return role is "Admin" or "Worker" or "Employer";
    }
}