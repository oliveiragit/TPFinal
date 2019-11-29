using SQLite;

namespace TPLivros.Data
{
    public interface IDependencyServiceSQLite
    {
        SQLiteConnection GetConexao();
    }
}
