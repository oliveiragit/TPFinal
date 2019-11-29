using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPLivros.ViewModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TPLivros.View.Livro
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class NovoLivroView : ContentPage
	{
        public static LivroViewModel AlunoVM { get; set; }
        public NovoLivroView ()
		{
			InitializeComponent ();
		}
	}
}