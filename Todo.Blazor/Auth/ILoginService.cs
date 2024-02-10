namespace Todo.Blazor.Auth
{
    public interface ILoginService
    {
        Task Login(string token);
        Task Logout();
    }
}
