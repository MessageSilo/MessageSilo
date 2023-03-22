namespace MessageSilo.SiloCTL
{
    internal class ConfigReader
    {
        public List<string> FileContents = new List<string>();

        public ConfigReader(string path)
        {
            FileAttributes attr = File.GetAttributes(path);

            if (attr.HasFlag(FileAttributes.Directory))
            {
                string[] filePaths = Directory.GetFiles(path, "*.yaml",
                                         SearchOption.TopDirectoryOnly);

                foreach (var filePath in filePaths)
                {
                    FileContents.AddRange(readFileContent(filePath));
                }
            }
            else
                FileContents.AddRange(readFileContent(path));

        }

        private IEnumerable<string> readFileContent(string filePath)
        {
            var result = File.ReadAllText(filePath);
            return result.Split("---", StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries);
        }
    }
}
