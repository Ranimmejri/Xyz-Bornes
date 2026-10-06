using Npgsql;
using Xyz.Bornes.Domaine.Entities;
using Xyz.Bornes.Domaine.Interfaces.DataAccess;

namespace Xyz.Bornes.Infrastructure.DataAccess.PostgreSQL;

public class ReservationRepository : IReservationRepository
{
    private readonly PostgreSqlConnectionFactory _connectionFactory;

    public ReservationRepository(
        PostgreSqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<Reservation?> GetReservationAsync(
        string rfid)
    {
        await using var connection =
            _connectionFactory.CreateConnection();

        await connection.OpenAsync();

        const string sql = """
            SELECT
                rfid,
                id_proprietaire,
                date_debut,
                date_fin
            FROM reservation
            WHERE rfid = @rfid;
            """;

        await using var command =
            new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue(
            "rfid",
            rfid);

        await using var reader =
            await command.ExecuteReaderAsync();

        if (await reader.ReadAsync())
        {
            return new Reservation
            {
                Rfid = reader.GetString(0),
                IdProprietaire = reader.GetInt32(1),
                DateDebut = reader.GetDateTime(2),
                DateFin = reader.IsDBNull(3)
                    ? null
                    : reader.GetDateTime(3)
            };
        }

        return null;
    }
}