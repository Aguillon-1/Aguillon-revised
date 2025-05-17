using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Microsoft.Data.SqlClient;

namespace CMS_Revised.Connections
{
    public class GoogleUserInfo
    {
        public required string Email { get; set; }
        public required string Name { get; set; }
    }

    class GAuthclass
    {
        private static readonly string DataStorePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
        private static readonly string UserIdFile = Path.Combine(DataStorePath, "last_user.txt");

        // Try auto-login: returns user info if credentials and DB last_login are valid, else null

        // Helper to get saved credential for a user
        private static async Task<UserCredential> GetSavedCredentialAsync(string userId)
        {
            string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Connections", "GAuth.json");
            using (var stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read))
            {
                return await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    new[] { "https://www.googleapis.com/auth/userinfo.email", "https://www.googleapis.com/auth/userinfo.profile" },
                    userId,
                    CancellationToken.None,
                    new FileDataStore(DataStorePath)
                );
            }
        }

        // Standard login flow, always updates last_login and saves userId
        public async Task<GoogleUserInfo> GetGoogleUserInfoAsync(Action<string> statusCallback)
        {
            statusCallback?.Invoke("Starting Google authentication...");
            string[] scopes = new string[] {
                "https://www.googleapis.com/auth/userinfo.email",
                "https://www.googleapis.com/auth/userinfo.profile",
            };

            UserCredential credential;
            string jsonPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Connections", "GAuth.json");
            using (var stream = new FileStream(jsonPath, FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    scopes,
                    "user", // always use "user" for this device
                    CancellationToken.None,
                    new FileDataStore(DataStorePath)
                );
            }

            var oauth2Service = new Oauth2Service(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Classroom Management System"
            });

            var userInfo = await oauth2Service.Userinfo.Get().ExecuteAsync();

            // Save userId for auto-login
            Directory.CreateDirectory(DataStorePath);
            File.WriteAllText(UserIdFile, userInfo.Email);

            // Update last_login in DB
            using (var conn = DatabaseConn.GetConnection())
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand("UPDATE users SET last_login = @Now WHERE email = @Email", conn))
                {
                    cmd.Parameters.AddWithValue("@Now", DateTime.UtcNow);
                    cmd.Parameters.AddWithValue("@Email", userInfo.Email);
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            statusCallback?.Invoke("Google authentication complete. Fetching user info...");
            return new GoogleUserInfo
            {
                Email = userInfo.Email,
                Name = userInfo.Name
            };
        }

        // For testing logout: clear Google credentials and userId
        public static async Task ClearGoogleCredentialsAsync()
        {
            if (Directory.Exists(DataStorePath))
            {
                Directory.Delete(DataStorePath, true);
            }
            await Task.CompletedTask;
        }
    }
}