using Xyz.Bornes.Domaine.Entities;

namespace Xyz.Bornes.Domaine.Interfaces.DataAccess;

public interface ICompteurRepository
{
    Task<List<Compteur>> GetCompteursAsync();
}