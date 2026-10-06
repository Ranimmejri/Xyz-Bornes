using Npgsql;
using Xyz.Bornes.Domaine.Entities;
using Xyz.Bornes.Domaine.Interfaces.DataAccess;

namespace Xyz.Bornes.Infrastructure.DataAccess.PostgreSQL;

public class TarifRepository : ITarifRepository
{
    private readonly PostgreSqlConnectionFactory _connectionFactory;

    public TarifRepository(
        PostgreSqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Tarif?> GetTarifActifAsync(
        string ressource)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();

        const string sql = """
            SELECT 
                t.id,
                t.ressource,
                t.version,
                t.date_effet,
                tr.id AS tranche_id,
                tr.v_debut,
                tr.v_fin,
                tr.prix
            FROM tarif t
            LEFT JOIN tranche tr
                ON tr.id_tarif = t.id
            WHERE t.ressource = @ressource
            ORDER BY t.version DESC, tr.v_debut;
            """;

        await using var command =
            new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "ressource",
            ressource);

        await using var reader =
            await command.ExecuteReaderAsync();

        Tarif? tarif = null;

        while (await reader.ReadAsync())
        {
            if (tarif == null)
            {
                tarif = new Tarif
                {
                    Id = reader.GetInt32(0),
                    Ressource = reader.GetString(1),
                    Version = reader.GetInt32(2),
                    DateEffet = reader.GetDateTime(3)
                };
            }

            if (!reader.IsDBNull(4))
            {
                tarif.Tranches.Add(
                    new Tranche
                    {
                        Id = reader.GetInt32(4),
                        VDebut = reader.GetDecimal(5),
                        VFin = reader.IsDBNull(6)
                            ? null
                            : reader.GetDecimal(6),
                        Prix = reader.GetDecimal(7),
                        IdTarif = tarif.Id
                    });
            }
        }

        return tarif;
    }
}