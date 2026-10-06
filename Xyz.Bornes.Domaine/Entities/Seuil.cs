namespace Xyz.Bornes.Domaine.Entities;

public class Seuil
{
    public int Id { get; set; }

    public string Type { get; set; } = string.Empty;

    public decimal Valeur { get; set; }

    public string Action { get; set; } = string.Empty;
}