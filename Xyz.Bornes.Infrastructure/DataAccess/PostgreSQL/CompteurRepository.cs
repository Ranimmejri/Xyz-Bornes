using Npgsql;
using Xyz.Bornes.Domaine.Entities;
using Xyz.Bornes.Domaine.Interfaces.DataAccess;

namespace Xyz.Bornes.Infrastructure.DataAccess.PostgreSQL;

public class CompteurRepository : ICompteurRepository
{
    private readonly PostgreSqlConnectionFactory _connectionFactory;

    public CompteurRepository(
        PostgreSqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<Compteur>> GetCompteursAsync()
    {
        var compteurs = new List<Compteur>();

        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();

        const string sql = """
            SELECT
                id,
                ressource,
                unite
            FROM compteur
            ORDER BY id;
            """;

        await using var command =
            new NpgsqlCommand(sql, connection);

        await using var reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            compteurs.Add(new Compteur
            {
                Id = reader.GetInt32(0),
                Ressource = reader.GetString(1),
                Unite = reader.GetString(2)
            });
        }

        return compteurs;
    }
}