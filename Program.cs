using Admin; // Add this using directive
namespace CMS_Revised
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Set up error handling for the application
            Application.ThreadException += (sender, args) =>
            {
                MessageBox.Show($"An error occurred: {args.Exception.Message}",
                    "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            // Create and show the login form
            var loginForm = new LoginForm();
            loginForm.Show();

            // Start the message loop without a main form
            Application.Run();
        }
    }
}