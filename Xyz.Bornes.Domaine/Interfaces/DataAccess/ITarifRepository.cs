using Xyz.Bornes.Domaine.Entities;

namespace Xyz.Bornes.Domaine.Interfaces.DataAccess;

public interface ITarifRepository
{
    Task<Tarif?> GetTarifActifAsync(string ressource);
}