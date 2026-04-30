using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Net.Http;
using System.Windows;
using AINotesHub.Shared.DTOs;
using AINotesHub.WPF;
using AINotesHub.WPF.Factories;
using AINotesHub.WPF.Helpers;
using AINotesHub.WPF.Properties;
using AINotesHub.WPF.Services;
using AINotesHub.WPF.ViewModels;
using AINotesHub.WPF.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http; // Add this using directive
using Serilog;


namespace AINotesHub.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider _serviceProvider { get; private set; }
        //public static ColorStateViewModel ColorState { get; private set; } = null!;
        public static NotesApiService NotesService { get; private set; }
        protected override async void OnStartup(StartupEventArgs e)
        {
            Log.Information("AINotesHub WPF started.");
            base.OnStartup(e);

            var services = new ServiceCollection();
            //DI Container
            //├── Services   → Logic
            //├── ViewModels → UI Brain
            //└── Views      → UI Screen
            // ===============================
            // ✅ SERVICES (Business / API)
            // ===============================

            // ✅ Register Services
            // A. Services (Logic / API / DB)
            services.AddHttpClient<INotesService, NotesApiService>();
            services.AddSingleton<INoteColorService, NoteColorService>();

            //// Web API/AI Services (with HttpClient)
            services.AddHttpClient<NotesApiService>();
            services.AddHttpClient<AIService>();

            // ✅ Register ViewModels
            // ✅ ViewModels (MVVM Brain)
            // ===============================

            // Main / App-Level
            services.AddSingleton<MainViewModel>();//ONE instance for the entire app lifetime
            services.AddSingleton<ColorStateViewModel>();// App brain
            services.AddSingleton<SidebarViewModel>();

            // UI / Page-Level
            services.AddTransient<BaseViewModel>();
            services.AddTransient<NoteViewModel>();
            services.AddTransient<UserViewModel>(); //NEW instance every time it’s requested//// UI element
            services.AddTransient<NoteDetailsDialogViewModel>();
            services.AddTransient<ProfileImageViewModel>();

            // ===============================
            // ✅ Views (Windows / UserControls)
            // ===============================
            services.AddTransient<SidebarView>();
            services.AddTransient<NoteEditorView>();
            services.AddTransient<LoginWindow>();
            services.AddTransient<MainWindow>();
            services.AddTransient<NoteEditorWindow>();
            services.AddTransient<ProfileImageView>();

            //Factory
            services.AddSingleton<NoteViewModelFactory>();
            services.AddTransient<NoteDetailsDialogViewModelFactory>();

            _serviceProvider = services.BuildServiceProvider();


            // 🧠 Auto-login check
            // 🧠 Restore saved user session (if any)
            string savedUsername = Settings.Default.Username;
            string savedRole = Settings.Default.Role;
            string savedUserId = Settings.Default.UserId;
            string savedToken = Settings.Default.JwtToken;
            string savedEmail = Settings.Default.Email;
            DateTime SavedExpirytime = Settings.Default.ExpiryTime;

            if (!string.IsNullOrEmpty(savedUsername) && !string.IsNullOrEmpty(savedRole))
            {
                // ✅ Restore session
                AppSession.Username = savedUsername;
                AppSession.Role = savedRole;
                AppSession.Email = savedEmail;   // 👈 ADD THIS
                AppSession.UserId = Guid.TryParse(savedUserId, out var userId) ? userId : Guid.Empty;
                AppSession.JwtToken = savedToken;
                AppSession.ExpiryTime = SavedExpirytime;

                // ✅ No need to assign IsAuthenticated manually
                // var notesService = new NotesApiService(AppSession.JwtToken);

                // ✅ Create service with injected token// After successful login
                var client = new HttpClient
                {
                    BaseAddress = new Uri("https://localhost:44357/")
                };

                // 1️⃣ Create shared state ONCE
                var ColorState = new ColorStateViewModel();

                // 2️⃣ Create other dependencies
                NotesService = new NotesApiService(client);
                await NotesService.SetJwtToken(AppSession.JwtToken);

                var noteColorService = new NoteColorService();

                // 3️⃣ Pass SAME colorState to everyone

                //new MainWindow(NotesService, savedUsername, savedRole, savedEmail).Show();
                var main = _serviceProvider.GetRequiredService<MainWindow>();
                main.Show();
                //this.Close();  // close only after success

            }
            else
            {
                var login = _serviceProvider.GetRequiredService<LoginWindow>();
                login.Show();
                //new LoginWindow().Show();
            }

            // ✅ If user already logged in, open MainWindow directly
            //if (AppSession.IsAuthenticated)
            //{
            //    // 🧠 Create your service here
            //    var notesService = new NotesApiService(AppSession.JwtToken);

            //    // new MainWindow().Show();
            //    var mainWindow = new MainWindow(notesService);
            //    mainWindow.Show();
            //}
            //else
            //{
            //    // ✅ Otherwise show LoginWindow first
            //    new LoginWindow().Show();
            //}
        }
        protected override void OnExit(ExitEventArgs e)
        {
            Log.Information("AINotesHub WPF closed.");
            Log.CloseAndFlush();
            base.OnExit(e);
        }
    }

}
