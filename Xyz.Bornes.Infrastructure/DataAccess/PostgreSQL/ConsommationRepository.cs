using Npgsql;
using Xyz.Bornes.Domaine.Entities;
using Xyz.Bornes.Domaine.Interfaces.DataAccess;

namespace Xyz.Bornes.Infrastructure.DataAccess.PostgreSQL;

public class ConsommationRepository : IConsommationRepository
{
    private readonly PostgreSqlConnectionFactory _connectionFactory;

    public ConsommationRepository(
        PostgreSqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task AjouterAsync(
        Consommation consommation)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();

        const string sql = """
            INSERT INTO consommation
            (
                date_consommation,
                ressource,
                quantite,
                prix,
                id_compteur,
                rfid,
                id_tarif
            )
            VALUES
            (
                @date_consommation,
                @ressource,
                @quantite,
                @prix,
                @id_compteur,
                @rfid,
                @id_tarif
            );
            """;

        await using var command =
            new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "date_consommation",
            consommation.DateConsommation);

        command.Parameters.AddWithValue(
            "ressource",
            consommation.Ressource);

        command.Parameters.AddWithValue(
            "quantite",
            consommation.Quantite);

        command.Parameters.AddWithValue(
            "prix",
            consommation.Prix);

        command.Parameters.AddWithValue(
            "id_compteur",
            consommation.IdCompteur);

        command.Parameters.AddWithValue(
            "rfid",
            consommation.Rfid);

        command.Parameters.AddWithValue(
            "id_tarif",
            consommation.IdTarif);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<List<Consommation>> GetHistoriqueAsync(
        string rfid)
    {
        var consommations = new List<Consommation>();

        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();

        const string sql = """
            SELECT
                id,
                date_consommation,
                ressource,
                quantite,
                prix,
                id_compteur,
                rfid,
                id_tarif
            FROM consommation
            WHERE rfid = @rfid
            ORDER BY date_consommation DESC;
            """;

        await using var command =
            new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "rfid",
            rfid);

        await using var reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            consommations.Add(new Consommation
            {
                Id = reader.GetInt32(0),
                DateConsommation = reader.GetDateTime(1),
                Ressource = reader.GetString(2),
                Quantite = reader.GetDecimal(3),
                Prix = reader.GetDecimal(4),
                IdCompteur = reader.GetInt32(5),
                Rfid = reader.GetString(6),
                IdTarif = reader.GetInt32(7)
            });
        }

        return consommations;
    }
}