using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TPLivros;
using TPLivros.Model;
using Xamarin.Forms;
using static TPLivros.Model.Livro;

namespace TPLivros.ViewModel { 
    public class LivroViewModel : INotifyPropertyChanged
    {
        #region Propriedades

        public Livro LivroModel { get; set; }
        public string Nome { get; set; }

        private Livro selecionado;
        public Livro Selecionado
        {
            get { return selecionado; }
            set
            {
                selecionado = value as Livro;
                EventPropertyChanged();
            }
        }

        private string pesquisaPorNome;
        public string PesquisaPorNome
        {
            get { return pesquisaPorNome; }
            set
            {
                if (value == pesquisaPorNome) return;

                pesquisaPorNome = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PesquisaPorNome)));
                AplicarFiltro();
            }
        }

        public List<Livro> CopiaListaLivros;
        public ObservableCollection<Livro> Livros { get; set; } = new ObservableCollection<Livro>();

        // UI Events
        public OnAdicionarLivroCMD OnAdicionarLivroCMD { get; }
        public OnEditarLivroCMD OnEditarLivroCMD { get; }
        public OnDeleteLivroCMD OnDeleteLivroCMD { get; }
        public OnDetalhes OnDetalhes { get; }
        public ICommand OnSairCMD { get; private set; }
        public ICommand OnNovoCMD { get; private set; }

        #endregion

        public LivroViewModel()
        {
            LivroRepository repository = LivroRepository.Instance;

            OnAdicionarLivroCMD = new OnAdicionarLivroCMD(this);
            OnEditarLivroCMD = new OnEditarLivroCMD(this);
            OnDeleteLivroCMD = new OnDeleteLivroCMD(this);
            OnDetalhes = new OnDetalhes(this);
            OnSairCMD = new Command(OnSair);
            OnNovoCMD = new Command(OnNovo);

            CopiaListaLivros = new List<Livro>();
            Carregar();
        }

        public void Carregar()
        {
            CopiaListaLivros = LivroRepository.GetLivros().ToList();
            AplicarFiltro();
        }

        private void AplicarFiltro()
        {
            if (pesquisaPorNome == null)
                pesquisaPorNome = "";

            var resultado = CopiaListaLivros.Where(n => n.Nome.ToLowerInvariant()
                                .Contains(PesquisaPorNome.ToLowerInvariant().Trim())).ToList();

            var removerDaLista = Livros.Except(resultado).ToList();
            foreach (var item in removerDaLista)
            {
                Livros.Remove(item);
            }

            for (int index = 0; index < resultado.Count; index++)
            {
                var item = resultado[index];
                if (index + 1 > Livros.Count || !Livros[index].Equals(item))
                    Livros.Insert(index, item);
            }
        }

        public void Adicionar(Livro paramLivro)
        {
            if ((paramLivro == null) || (string.IsNullOrWhiteSpace(paramLivro.Nome)))
                App.Current.MainPage.DisplayAlert("Atenção", "O campo nome é obrigatório", "OK");
            else if (LivroRepository.SalvarLivro(paramLivro) > 0)
                App.Current.MainPage.Navigation.PopAsync();
            else
                App.Current.MainPage.DisplayAlert("Falhou", "Desculpe, ocorreu um erro inesperado =(", "OK");
        }

        public async void Editar()
        {
            await App.Current.MainPage.Navigation.PushAsync(
                new View.Livro.NovoLivroView() { BindingContext = App.LivroVM });
        }

        public async void Remover()
        {
            if (await App.Current.MainPage.DisplayAlert("Atenção?",
                string.Format("Tem certeza que deseja remover o {0}?", Selecionado.Nome), "Sim", "Não"))
            {
                if (LivroRepository.RemoverLivro(Selecionado.Id) > 0)
                {
                    CopiaListaLivros.Remove(Selecionado);
                    Carregar();
                }
                else
                    await App.Current.MainPage.DisplayAlert(
                            "Falhou", "Desculpe, ocorreu um erro inesperado =(", "OK");
            }
        }
        public async void Detalhes()
        {
            await App.Current.MainPage.Navigation.PushAsync(
                new View.Livro.DetalheLivroView() { BindingContext = App.LivroVM });
        }

        private async void OnSair()
        {
            await App.Current.MainPage.Navigation.PopAsync();
        }

        private void OnNovo()
        {
            App.LivroVM.Selecionado = new Model.Livro();
            App.Current.MainPage.Navigation.PushAsync(
                new View.Livro.NovoLivroView() { BindingContext = App.LivroVM });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void EventPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
      
    }

    public class OnAdicionarLivroCMD : ICommand
    {
        private LivroViewModel livroVM;
        public OnAdicionarLivroCMD(LivroViewModel paramVM)
        {
            livroVM = paramVM;
        }
        public event EventHandler CanExecuteChanged;
        public void AdicionarCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        public bool CanExecute(object parameter) => true;
        public void Execute(object parameter)
        {
            livroVM.Adicionar(parameter as Livro);
        }
    }

    public class OnEditarLivroCMD : ICommand
    {
        private LivroViewModel livroVM;
        public OnEditarLivroCMD(LivroViewModel paramVM)
        {
            livroVM = paramVM;
        }
        public event EventHandler CanExecuteChanged;
        public void EditarCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        public bool CanExecute(object parameter) => (parameter != null);
        public void Execute(object parameter)
        {
            App.LivroVM.Selecionado = parameter as Livro;
            livroVM.Editar();
        }
    }

    public class OnDeleteLivroCMD : ICommand
    {
        private LivroViewModel livroVM;
        public OnDeleteLivroCMD(LivroViewModel paramVM)
        {
            livroVM = paramVM;
        }
        public event EventHandler CanExecuteChanged;
        public void DeleteCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        public bool CanExecute(object parameter) => (parameter != null);
        public void Execute(object parameter)
        {
            App.LivroVM.Selecionado = parameter as Livro;
            livroVM.Remover();
        }
    }
    public class OnDetalhes : ICommand
    {
        private LivroViewModel livroVM;
        public OnDetalhes(LivroViewModel paramVM)
        {
            livroVM = paramVM;
        }
        public event EventHandler CanExecuteChanged;
        public void EditarCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        public bool CanExecute(object parameter) => (parameter != null);
        public void Execute(object parameter)
        {
            App.LivroVM.Selecionado = parameter as Livro;
            livroVM.Detalhes();
        }
    }
}
