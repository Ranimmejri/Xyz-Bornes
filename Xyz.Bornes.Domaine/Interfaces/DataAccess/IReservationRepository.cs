using Xyz.Bornes.Domaine.Entities;

namespace Xyz.Bornes.Domaine.Interfaces.DataAccess;

public interface IReservationRepository
{
    Task<Reservation?> GetReservationAsync(string rfid);
}