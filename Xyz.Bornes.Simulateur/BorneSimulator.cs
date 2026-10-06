using Xyz.Bornes.Domaine.Entities;

namespace Xyz.Bornes.Simulateur;

public class BorneSimulator
{
    public string Id { get; }

    public Compteur CompteurElectricite { get; }
    public Compteur CompteurEau { get; }

    public decimal ConsommationElectricite { get; private set; }
    public decimal ConsommationEau { get; private set; }

    public decimal PrixElectricite { get; private set; }
    public decimal PrixEau { get; private set; }

    public bool ConsommationActive { get; private set; } = true;

    public Tarif TarifElectricite { get; }
    public Tarif TarifEau { get; }

    public List<Seuil> Seuils { get; }

    private readonly HashSet<int> _seuilsDeclenches = new();

    public BorneSimulator(
        string id,
        Compteur compteurElectricite,
        Compteur compteurEau,
        Tarif tarifElectricite,
        Tarif tarifEau,
        List<Seuil> seuils)
    {
        Id = id;

        CompteurElectricite = compteurElectricite;
        CompteurEau = compteurEau;

        TarifElectricite = tarifElectricite;
        TarifEau = tarifEau;

        Seuils = seuils;
    }

    public decimal AjouterElectricite(decimal quantite)
    {
        if (!ConsommationActive)
            return 0;

        decimal ancienPrix = PrixElectricite;

        ConsommationElectricite += quantite;

        PrixElectricite =
            CalculateurTarif.CalculerPrix(
                ConsommationElectricite,
                TarifElectricite);

        return Math.Round(
            PrixElectricite - ancienPrix,
            3);
    }

    public decimal AjouterEau(decimal quantite)
    {
        if (!ConsommationActive)
            return 0;

        decimal ancienPrix = PrixEau;

        ConsommationEau += quantite;

        PrixEau =
            CalculateurTarif.CalculerPrix(
                ConsommationEau,
                TarifEau);

        return Math.Round(
            PrixEau - ancienPrix,
            3);
    }

    public void ArreterConsommation()
    {
        ConsommationActive = false;
    }

    public Alerte? VerifierSeuils()
    {
        decimal prixTotal =
            PrixElectricite + PrixEau;

        foreach (var seuil in Seuils
            .Where(s => s.Type == "MONTANT")
            .OrderBy(s => s.Valeur))
        {
            if (_seuilsDeclenches.Contains(seuil.Id))
                continue;

            if (prixTotal >= seuil.Valeur)
            {
                _seuilsDeclenches.Add(seuil.Id);

                if (seuil.Action == "COUPER")
                {
                    ArreterConsommation();
                }

                return new Alerte
                {
                    BorneId = Id,
                    Type = seuil.Type,
                    Seuil = seuil.Valeur,
                    Valeur = prixTotal,
                    Action = seuil.Action,
                    Timestamp = DateTime.Now
                };
            }
        }

        return null;
    }
}