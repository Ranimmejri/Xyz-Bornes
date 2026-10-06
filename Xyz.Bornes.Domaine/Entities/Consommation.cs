namespace Xyz.Bornes.Domaine.Entities;

public class Consommation
{
    public int Id { get; set; }

    public DateTime DateConsommation { get; set; }

    public string Ressource { get; set; } = string.Empty;

    public decimal Quantite { get; set; }

    public decimal Prix { get; set; }

    public int IdCompteur { get; set; }

    public string Rfid { get; set; } = string.Empty;

    public int IdTarif { get; set; }
}
