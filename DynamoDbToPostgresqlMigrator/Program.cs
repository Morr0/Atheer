using System;
using System.Threading.Tasks;

namespace DynamoDbToPostgresqlMigrator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var articles = await LoadArticles();
            var users = await LoadUsers();
            await WriteToSql
        }
        
        static 
    }
}