using System.Text.Json;
using Xyz.Bornes.Domaine.Entities;

namespace Xyz.Bornes.Simulateur;

public static class MessageMqtt
{
    public static string CreerTelemetrie(
        BorneSimulator borne,
        decimal electriciteInstantanee,
        decimal eauInstantanee)
    {
        var message = new
        {
            borneId = borne.Id,
            timestamp = DateTime.Now,
            electricite = new
            {
                instantanee = electriciteInstantanee,
                cumulee = borne.ConsommationElectricite,
                prix = borne.PrixElectricite,
                unite = "kWh"
            },
            eau = new
            {
                instantanee = eauInstantanee,
                cumulee = borne.ConsommationEau,
                prix = borne.PrixEau,
                unite = "m3"
            },
            prixTotal = borne.PrixElectricite + borne.PrixEau
        };

        return JsonSerializer.Serialize(message);
    }

    public static string CreerAlerte(Alerte alerte)
    {
        return JsonSerializer.Serialize(alerte);
    }

    public static string CreerEtat(
        BorneSimulator borne)
    {
        string etat =
            borne.ConsommationActive
                ? "ACTIVE"
                : "COUPEE";

        var message = new
        {
            borneId = borne.Id,
            timestamp = DateTime.Now,
            etat = etat
        };

        return JsonSerializer.Serialize(message);
    }
}