using System;

namespace TixNova_Final
{
    // The "static" keyword means this class exists globally while the app runs
    public static class UserSession
    {
        // This variable will hold the name of whoever successfully logs in
        public static string CurrentUsername { get; set; }
    }
}