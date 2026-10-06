namespace Xyz.Bornes.Domaine.Entities;

public class Tranche
{
    public int Id { get; set; }

    public decimal VDebut { get; set; }

    public decimal? VFin { get; set; }

    public decimal Prix { get; set; }

    public int IdTarif { get; set; }
}