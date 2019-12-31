namespace Executables
{
    public static class Config
    {
        public static string GetTestDatabaseConnectionString(string dbName)
        {
            return $"Data Source=DESKTOP-VSBO5TE\\SQLEXPRESS;Initial Catalog=MusicAutomatedTest-{dbName};Integrated Security=True";
        }
    }
}
