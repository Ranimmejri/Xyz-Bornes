using Xyz.Bornes.Domaine.Entities;

namespace Xyz.Bornes.Simulateur;

public static class CalculateurTarif
{
    public static decimal CalculerPrix(
        decimal quantite,
        Tarif tarif)
    {
        decimal prixTotal = 0;

        foreach (var tranche in tarif.Tranches.OrderBy(t => t.VDebut))
        {
            if (quantite <= tranche.VDebut)
                continue;

            decimal quantiteDansTranche;

            if (tranche.VFin == null)
            {
                quantiteDansTranche = quantite - tranche.VDebut;
            }
            else
            {
                var limite = Math.Min(quantite, tranche.VFin.Value);

                quantiteDansTranche =
                    limite - tranche.VDebut;
            }

            if (quantiteDansTranche > 0)
            {
                prixTotal +=
                    quantiteDansTranche * tranche.Prix;
            }
        }

        return Math.Round(prixTotal, 3);
    }
}