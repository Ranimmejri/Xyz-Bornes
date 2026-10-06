using Xyz.Bornes.Domaine.Entities;

namespace Xyz.Bornes.Domaine.Interfaces.DataAccess;

public interface ISeuilRepository
{
    Task<List<Seuil>> GetSeuilsAsync();
}