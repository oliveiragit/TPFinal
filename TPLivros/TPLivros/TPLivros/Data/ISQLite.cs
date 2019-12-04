using SQLite;

namespace TPLivros.Data
{
    public interface ISQLite
    {
        SQLiteConnection GetConexao();
    }
}
