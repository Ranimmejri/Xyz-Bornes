namespace Xyz.Bornes.Domaine.Interfaces.Transport;

public interface ITransport
{
    Task ConnecterAsync();

    Task PublierAsync(
        string topic,
        string message);

    Task DeconnecterAsync();
}