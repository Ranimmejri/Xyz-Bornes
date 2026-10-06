namespace Xyz.Bornes.Domaine.Entities;

public class Reservation
{
    public string Rfid { get; set; } = string.Empty;

    public int IdProprietaire { get; set; }

    public DateTime DateDebut { get; set; }

    public DateTime? DateFin { get; set; }
}