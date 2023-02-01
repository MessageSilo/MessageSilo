namespace MessageSilo.Features.Connection
{
    public interface IMessageCorrector
    {
        string Correct(string message, string currectorFuncBody);
    }
}
