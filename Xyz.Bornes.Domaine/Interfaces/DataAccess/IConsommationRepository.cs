using Xyz.Bornes.Domaine.Entities;

namespace Xyz.Bornes.Domaine.Interfaces.DataAccess;

public interface IConsommationRepository
{
    Task AjouterAsync(Consommation consommation);

    Task<List<Consommation>> GetHistoriqueAsync(
        string rfid);
}