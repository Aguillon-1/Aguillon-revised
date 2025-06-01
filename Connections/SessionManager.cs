using System;
using System.IO;
using System.Text.Json;

namespace CMS_Revised.User_Interface
{
    public static class SessionManager
    {
        private static readonly string DataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        private static readonly string SessionFilePath = Path.Combine(DataDir, "session.json");

        public static int UserId { get; private set; }
        public static string Email { get; private set; }
        public static string Role { get; private set; }
        public static string FirstName { get; private set; }
        public static string MiddleName { get; private set; }
        public static string LastName { get; private set; }

        public static bool IsLoggedIn => UserId > 0;

        public static void StartSession(int userId, string email, string role, string firstName, string middleName, string lastName)
        {
            UserId = userId;
            Email = email;
            Role = role;
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            SaveSession();
        }

        public static void EndSession()
        {
            UserId = 0;
            Email = null;
            Role = null;
            FirstName = null;
            MiddleName = null;
            LastName = null;
            if (File.Exists(SessionFilePath))
                File.Delete(SessionFilePath);
        }

        public static void SaveSession()
        {
            if (!Directory.Exists(DataDir))
                Directory.CreateDirectory(DataDir);

            var session = new
            {
                UserId,
                Email,
                Role,
                FirstName,
                MiddleName,
                LastName
            };
            var json = JsonSerializer.Serialize(session);
            File.WriteAllText(SessionFilePath, json);
        }

        public static bool LoadSession()
        {
            if (!File.Exists(SessionFilePath))
                return false;

            try
            {
                var json = File.ReadAllText(SessionFilePath);
                var session = JsonSerializer.Deserialize<SessionData>(json);
                if (session != null && session.UserId > 0)
                {
                    UserId = session.UserId;
                    Email = session.Email;
                    Role = session.Role;
                    FirstName = session.FirstName;
                    MiddleName = session.MiddleName;
                    LastName = session.LastName;
                    return true;
                }
            }
            catch
            {
                // Ignore errors, treat as no session
            }
            return false;
        }

        private class SessionData
        {
            public int UserId { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
        }
    }
}