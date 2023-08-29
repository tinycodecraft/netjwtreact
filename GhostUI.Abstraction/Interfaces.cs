﻿namespace GhostUI.Abstraction
{
    public class Interfaces
    {
        public interface ILanguageService
        {
            public string LanguageId { get; }
        }

        public interface ILoginError
        {
            public string Error { get; }
            public bool NeedNew { get; }
        }


        public interface IJwtManager
        {
            Task<ILoginError?> Authenticate(string username, string password);
            Task<string> Register(string username, string password);
        }
    }


}