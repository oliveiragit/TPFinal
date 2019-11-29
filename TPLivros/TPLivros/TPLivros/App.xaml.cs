using System;
using TPLivros.ViewModel;
using TPLivros;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace TPLivros
{
     public partial class App : Application
    {
        #region ViewModels
        public static LivroViewModel LivroVM { get; set; }
        #endregion

        public App()
        {
            InitializeComponent();
            InitializeApplication();

            MainPage = new NavigationPage(new View.Livro.MainPage { BindingContext = App.LivroVM });
        }

        private void InitializeApplication()
        {
            if (LivroVM == null) LivroVM = new LivroViewModel();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
