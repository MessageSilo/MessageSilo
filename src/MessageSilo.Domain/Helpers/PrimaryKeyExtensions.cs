namespace MessageSilo.Domain.Helpers
{
    public static class PrimaryKeyExtensions
    {
        public static (string userId, string name, string scaleSet) Explode(this string input)
        {
            var splitted1 = input.Split('|');
            var userId = splitted1[0];

            var splitted2 = splitted1[1].Split('#');
            var name = splitted2[0];
            var scaleSet = splitted2[1];

            return (userId, name, scaleSet);
        }
    }
}
