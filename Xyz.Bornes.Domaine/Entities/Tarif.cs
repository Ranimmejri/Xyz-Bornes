namespace Xyz.Bornes.Domaine.Entities;

public class Tarif
{
    public int Id { get; set; }

    public string Ressource { get; set; } = string.Empty;

    public int Version { get; set; }

    public DateTime DateEffet { get; set; }

    public List<Tranche> Tranches { get; set; } = new();
}