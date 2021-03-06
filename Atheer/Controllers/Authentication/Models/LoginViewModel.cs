﻿namespace Atheer.Controllers.Authentication.Models
{
    public class LoginViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool ShowAttemptsLeft { get; set; }
        public int AttemptsLeft { get; set; }
        public bool EmphasizeUsername { get; set; }
    }
}