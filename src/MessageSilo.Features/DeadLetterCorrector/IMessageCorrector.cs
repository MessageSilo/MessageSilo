namespace MessageSilo.Features.DeadLetterCorrector
{
    public interface IMessageCorrector
    {
        string Correct(string message, string currectorFuncBody);
    }
}
