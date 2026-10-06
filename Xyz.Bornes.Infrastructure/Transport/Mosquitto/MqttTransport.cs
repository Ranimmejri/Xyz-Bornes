using MQTTnet;
using Xyz.Bornes.Domaine.Interfaces.Transport;

namespace Xyz.Bornes.Infrastructure.Transport.Mosquitto;

public class MqttTransport : ITransport
{
    private readonly IMqttClient _client;
    private readonly string _host;
    private readonly int _port;

    public MqttTransport(
        string host = "localhost",
        int port = 1883)
    {
        _host = host;
        _port = port;

        var factory = new MqttClientFactory();

        _client = factory.CreateMqttClient();
    }

    public async Task ConnecterAsync()
    {
        var options =
            new MqttClientOptionsBuilder()
                .WithTcpServer(_host, _port)
                .WithClientId(
                    $"Xyz-Bornes-{Guid.NewGuid()}")
                .Build();

        await _client.ConnectAsync(options);

        Console.WriteLine(
            "Connexion MQTT réussie.");
    }

    public async Task PublierAsync(
        string topic,
        string message)
    {
        if (!_client.IsConnected)
        {
            throw new InvalidOperationException(
                "Le client MQTT n'est pas connecté.");
        }

        var mqttMessage =
            new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(message)
                .Build();

        await _client.PublishAsync(
            mqttMessage);
    }

    public async Task DeconnecterAsync()
    {
        if (_client.IsConnected)
        {
            await _client.DisconnectAsync();

            Console.WriteLine(
                "Déconnexion MQTT.");
        }
    }
}