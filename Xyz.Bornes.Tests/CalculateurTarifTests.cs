using Xyz.Bornes.Domaine.Entities;
using Xyz.Bornes.Simulateur;

namespace Xyz.Bornes.Tests;

public class CalculateurTarifTests
{
    [Fact]
    public void CalculerPrix_5Kwh_Retourne1250()
    {
        var tarif = new Tarif
        {
            Id = 1,
            Ressource = "ELECTRICITE",
            Version = 1,
            DateEffet = new DateTime(2026, 1, 1),
            Tranches =
            [
                new Tranche
                {
                    VDebut = 0,
                    VFin = 10,
                    Prix = 0.250m
                },
                new Tranche
                {
                    VDebut = 10,
                    VFin = 30,
                    Prix = 0.300m
                },
                new Tranche
                {
                    VDebut = 30,
                    VFin = null,
                    Prix = 0.380m
                }
            ]
        };

        var resultat =
            CalculateurTarif.CalculerPrix(5, tarif);

        Assert.Equal(1.250m, resultat);
    }

    [Fact]
    public void CalculerPrix_15Kwh_Retourne4000()
    {
        var tarif = new Tarif
        {
            Id = 1,
            Ressource = "ELECTRICITE",
            Version = 1,
            DateEffet = new DateTime(2026, 1, 1),
            Tranches =
            [
                new Tranche
                {
                    VDebut = 0,
                    VFin = 10,
                    Prix = 0.250m
                },
                new Tranche
                {
                    VDebut = 10,
                    VFin = 30,
                    Prix = 0.300m
                },
                new Tranche
                {
                    VDebut = 30,
                    VFin = null,
                    Prix = 0.380m
                }
            ]
        };

        var resultat =
            CalculateurTarif.CalculerPrix(15, tarif);

        Assert.Equal(4.000m, resultat);
    }

    [Fact]
    public void CalculerPrix_35Kwh_Retourne10400()
    {
        var tarif = new Tarif
        {
            Id = 1,
            Ressource = "ELECTRICITE",
            Version = 1,
            DateEffet = new DateTime(2026, 1, 1),
            Tranches =
            [
                new Tranche
                {
                    VDebut = 0,
                    VFin = 10,
                    Prix = 0.250m
                },
                new Tranche
                {
                    VDebut = 10,
                    VFin = 30,
                    Prix = 0.300m
                },
                new Tranche
                {
                    VDebut = 30,
                    VFin = null,
                    Prix = 0.380m
                }
            ]
        };

        var resultat =
            CalculateurTarif.CalculerPrix(35, tarif);

        Assert.Equal(10.400m, resultat);
    }
}