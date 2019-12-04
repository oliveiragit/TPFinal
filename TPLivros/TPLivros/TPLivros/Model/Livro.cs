using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TPLivros.Data;
using Xamarin.Forms;

namespace TPLivros.Model
{
    public class Livro
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Nome { get; set; }
        public string ISBN { get; set; }
        public string NomeAutor { get; set; }
        public string EmailAutor { get; set; }
    }
    public class LivroRepository
    {
        private LivroRepository() { }

        private static SQLiteConnection database;
        private static readonly LivroRepository instance = new LivroRepository();
        public static LivroRepository Instance
        {
            get
            {
                if (database == null)
                {
                    database = DependencyService.Get<ISQLite>().GetConexao();
                    database.CreateTable<Livro>();
                }
                return instance;
            }
        }

        static object locker = new object();

        public static int SalvarLivro(Livro livro)
        {
            lock (locker)
            {
                if (livro.Id != 0)
                {
                    database.Update(livro);
                    return livro.Id;
                }
                else return database.Insert(livro);
            }
        }

        public static IEnumerable<Livro> GetLivros()
        {
            lock (locker)
            {
                return (from c in database.Table<Livro>()
                        select c).ToList();
            }
        }

        public static Livro GetLivros(int Id)
        {
            lock (locker)
            {
                // return database.Query<Livro>("SELECT * FROM [Livro] WHERE [Id] = " + Id).FirstOrDefault();
                return database.Table<Livro>().Where(c => c.Id == Id).FirstOrDefault();
            }
        }

        public static int RemoverLivro(int Id)
        {
            lock (locker)
            {
                return database.Delete<Livro>(Id);
            }
        }
    }
}