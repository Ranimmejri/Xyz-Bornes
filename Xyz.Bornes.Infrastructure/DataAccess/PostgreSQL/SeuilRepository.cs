using Npgsql;
using Xyz.Bornes.Domaine.Entities;
using Xyz.Bornes.Domaine.Interfaces.DataAccess;

namespace Xyz.Bornes.Infrastructure.DataAccess.PostgreSQL;

public class SeuilRepository : ISeuilRepository
{
    private readonly PostgreSqlConnectionFactory _connectionFactory;

    public SeuilRepository(
        PostgreSqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<Seuil>> GetSeuilsAsync()
    {
        var seuils = new List<Seuil>();

        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();

        const string sql = """
            SELECT
                id,
                type,
                valeur,
                action
            FROM seuil
            ORDER BY valeur;
            """;

        await using var command =
            new NpgsqlCommand(sql, connection);

        await using var reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            seuils.Add(new Seuil
            {
                Id = reader.GetInt32(0),
                Type = reader.GetString(1),
                Valeur = reader.GetDecimal(2),
                Action = reader.GetString(3)
            });
        }

        return seuils;
    }
}