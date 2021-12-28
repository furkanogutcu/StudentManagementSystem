namespace StudentManagementSystem.Core.DataAccess.Sql
{
    public static class DbContext
    {
        public static string ConnectionString { get; } = "Datasource=127.0.0.1;Port=3306;Username=YOUR_DB_USERNAME;Password=YOUR_DB_PASSWORD;Database=student_management;";
    }
}
