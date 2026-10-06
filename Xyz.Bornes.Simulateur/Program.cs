using Xyz.Bornes.Infrastructure.DataAccess.PostgreSQL;
using Xyz.Bornes.Infrastructure.Transport.Mosquitto;
using Xyz.Bornes.Simulateur;
using Xyz.Bornes.Domaine.Entities;

string connectionString =
    Environment.GetEnvironmentVariable("XYZ_DB_CONNECTION")
    ?? throw new InvalidOperationException(
        "La variable XYZ_DB_CONNECTION n'est pas configurée.");
// ========================================
// Connexion PostgreSQL
// ========================================

var factory =
    new PostgreSqlConnectionFactory(connectionString);

// ========================================
// Repositories
// ========================================

var tarifRepository =
    new TarifRepository(factory);

var seuilRepository =
    new SeuilRepository(factory);

var compteurRepository =
    new CompteurRepository(factory);

var consommationRepository =
    new ConsommationRepository(factory);

// ========================================
// Chargement depuis PostgreSQL
// ========================================

var tarifElectricite =
    await tarifRepository.GetTarifActifAsync("ELECTRICITE");

var tarifEau =
    await tarifRepository.GetTarifActifAsync("EAU");

var seuils =
    await seuilRepository.GetSeuilsAsync();

Console.WriteLine("=== SEUILS CHARGÉS ===");

foreach (var seuil in seuils)
{
    Console.WriteLine(
        $"ID={seuil.Id} | " +
        $"Type={seuil.Type} | " +
        $"Valeur={seuil.Valeur:F3} | " +
        $"Action={seuil.Action}");
}

Console.WriteLine("======================");


var compteurs =
    await compteurRepository.GetCompteursAsync();

if (tarifElectricite == null ||
    tarifEau == null)
{
    Console.WriteLine(
        "Erreur : tarifs introuvables.");

    return;
}

var compteurElectricite =
    compteurs.FirstOrDefault(
        c => c.Ressource == "ELECTRICITE");

var compteurEau =
    compteurs.FirstOrDefault(
        c => c.Ressource == "EAU");

if (compteurElectricite == null ||
    compteurEau == null)
{
    Console.WriteLine(
        "Erreur : compteurs introuvables.");

    return;
}

// ========================================
// Connexion MQTT
// ========================================

var mqtt =
    new MqttTransport();

await mqtt.ConnecterAsync();

// ========================================
// Création des bornes
// ========================================

var b001 = new BorneSimulator(
    "B001",
    compteurElectricite,
    compteurEau,
    tarifElectricite,
    tarifEau,
    seuils);

var b002 = new BorneSimulator(
    "B002",
    compteurElectricite,
    compteurEau,
    tarifElectricite,
    tarifEau,
    seuils);

var b003 = new BorneSimulator(
    "B003",
    compteurElectricite,
    compteurEau,
    tarifElectricite,
    tarifEau,
    seuils);

// ========================================
// Threads
// ========================================

Thread threadB001 =
    new(() => SimulerBorne(
        b001,
        consommationRepository,
        mqtt));

Thread threadB002 =
    new(() => SimulerBorne(
        b002,
        consommationRepository,
        mqtt));

Thread threadB003 =
    new(() => SimulerBorne(
        b003,
        consommationRepository,
        mqtt));

// ========================================
// Démarrage
// ========================================

threadB001.Start();
threadB002.Start();
threadB003.Start();

// Attendre la fin des 3 simulations

threadB001.Join();
threadB002.Join();
threadB003.Join();

// ========================================
// Déconnexion MQTT
// ========================================

await mqtt.DeconnecterAsync();

Console.WriteLine(
    "Simulation terminée.");


// ========================================
// Simulation d'une borne
// ========================================

static void SimulerBorne(
    BorneSimulator borne,
    ConsommationRepository consommationRepository,
    MqttTransport mqtt)
{
    Random random = new();

    while (borne.ConsommationActive)
    {
        // ========================================
        // Génération de la consommation
        // ========================================

        decimal electricite =
            (decimal)(
                random.NextDouble() * 0.5 + 0.1);

        decimal eau =
            (decimal)(
                random.NextDouble() * 0.1 + 0.02);

        // ========================================
        // Ajout à la consommation cumulée
        // ========================================

        decimal prixElectricite =
            borne.AjouterElectricite(electricite);

        decimal prixEau =
            borne.AjouterEau(eau);

        // ========================================
        // Enregistrement électricité
        // ========================================

        var consommationElectricite =
            new Xyz.Bornes.Domaine.Entities.Consommation
            {
                DateConsommation = DateTime.Now,
                Ressource = "ELECTRICITE",
                Quantite = electricite,
                Prix = prixElectricite,
                IdCompteur = borne.CompteurElectricite.Id,
                Rfid = "RFID001",
                IdTarif = borne.TarifElectricite.Id
            };

        consommationRepository
            .AjouterAsync(consommationElectricite)
            .GetAwaiter()
            .GetResult();

        // ========================================
        // Enregistrement eau
        // ========================================

        var consommationEau =
            new Xyz.Bornes.Domaine.Entities.Consommation
            {
                DateConsommation = DateTime.Now,
                Ressource = "EAU",
                Quantite = eau,
                Prix = prixEau,
                IdCompteur = borne.CompteurEau.Id,
                Rfid = "RFID001",
                IdTarif = borne.TarifEau.Id
            };

        consommationRepository
            .AjouterAsync(consommationEau)
            .GetAwaiter()
            .GetResult();

        // ========================================
        // Prix total cumulé
        // ========================================

        decimal prixTotal =
            borne.PrixElectricite +
            borne.PrixEau;

        // ========================================
        // Vérification des seuils
        // ========================================

        Alerte? alerte =
        borne.VerifierSeuils();

        // ========================================
        // Publication MQTT de la télémétrie
        // ========================================

        string telemetrie =
            MessageMqtt.CreerTelemetrie(
                borne,
                electricite,
                eau);

        mqtt.PublierAsync(
                $"port/bornes/{borne.Id}/telemetrie",
                telemetrie)
            .GetAwaiter()
            .GetResult();

        // ========================================
        // Publication MQTT de l'alerte
        // ========================================

        if (alerte != null)
    {
        string messageAlerte =
            MessageMqtt.CreerAlerte(alerte);

        Console.WriteLine(
            $"🚨 ALERTE | {borne.Id} | " +
            $"Type={alerte.Type} | " +
            $"Seuil={alerte.Seuil:F3} DT | " +
            $"Valeur={alerte.Valeur:F3} DT | " +
            $"Action={alerte.Action}");

        mqtt.PublierAsync(
                $"port/bornes/{borne.Id}/alertes",
                messageAlerte)
            .GetAwaiter()
            .GetResult();

        string etat =
            MessageMqtt.CreerEtat(borne);

        mqtt.PublierAsync(
                $"port/bornes/{borne.Id}/etat",
                etat)
            .GetAwaiter()
            .GetResult();
    }

        // ========================================
        // Affichage console
        // ========================================

        Console.WriteLine(
    $"{DateTime.Now:HH:mm:ss} | " +
    $"{borne.Id} | " +
    $"Électricité: {borne.ConsommationElectricite:F3} kWh | " +
    $"Eau: {borne.ConsommationEau:F3} m3 | " +
    $"Prix total: {prixTotal:F3} DT | " +
    $"État: {(borne.ConsommationActive ? "ACTIVE" : "COUPÉE")}");

        Thread.Sleep(1000);
    }

}

