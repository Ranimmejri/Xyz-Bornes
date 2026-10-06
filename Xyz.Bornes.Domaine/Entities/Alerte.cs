namespace Xyz.Bornes.Domaine.Entities;

public class Alerte
{
    public string BorneId { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public decimal Seuil { get; set; }

    public decimal Valeur { get; set; }

    public string Action { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; }
}