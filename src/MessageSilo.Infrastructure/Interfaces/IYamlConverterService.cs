namespace MessageSilo.Infrastructure.Interfaces
{
    public interface IYamlConverterService
    {
        string Serialize<T>(T input);

        T Deserialize<T>(string input);
    }
}
