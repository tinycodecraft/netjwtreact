namespace GhostUI.Abstraction
{
    public class Interfaces
    {
        public interface ILanguageService
        {
            public string LanguageId { get; }
        }

        public interface IJwtManager
        {
            Task<string> Authenticate(string username, string password);
            Task<string> Register(string username, string password);
        }
    }


}