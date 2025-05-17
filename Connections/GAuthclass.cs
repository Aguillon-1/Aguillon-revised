using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using static System.Collections.Specialized.BitVector32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace CMS_Revised.Connections
{
    class GAuthclass
    {
        public async Task callGAuthAsync()
        {
            Debug.WriteLine("GAUTH STARTED");
            string[] scopes = new string[] {
                    "https://www.googleapis.com/auth/userinfo.email",
                    "https://www.googleapis.com/auth/userinfo.profile",
                };

            UserCredential credential;

            using (var stream = new FileStream("GAuth.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore("Data")
                );
            }

            // Now you can use the credential to make API calls
            // For example, to get user information:
            var oauth2Service = new Oauth2Service(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Classroom Management System"
            });

            var userInfo = await oauth2Service.Userinfo.Get().ExecuteAsync();

            var userEmail = userInfo.Email;
            var userName = userInfo.Name;

            DatabaseConn.GetConnection();

            // Display the user information
            MessageBox.Show($"\nEmail: {userEmail}\nName: {userName}");

            InsertionQueries insertionqueries = new InsertionQueries();
            insertionqueries.InsertDatafromgoogle(userName, userEmail);
        }
    }
}
