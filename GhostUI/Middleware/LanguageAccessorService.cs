﻿using GhostUI.Abstraction;
using Microsoft.AspNetCore.Http;
using System;
using static GhostUI.Abstraction.Interfaces;

namespace GhostUI.Middleware
{

    public class LanguageAccessorService : ILanguageService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public LanguageAccessorService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            var lang = _httpContextAccessor.HttpContext?.Request.Headers["Accept-Language"].ToString();
            LanguageId = string.IsNullOrEmpty(lang) ? "en-US" : lang;
        }
        public string LanguageId { get; }
    }
}
