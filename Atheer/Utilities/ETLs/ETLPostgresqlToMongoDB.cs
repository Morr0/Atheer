using System.Threading.Tasks;
using Atheer.Extensions;
using Atheer.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Atheer.Utilities.ETLs
{
    public static class ETLPostgresqlToMongoDB
    {
        public static void MigrateToMongo(Data originalDbContext, IMongoClient targetMongoClient, ILogger logger)
        {
            Task.Run(async () =>
            {
                logger.LogCritical("Begun postgresql to mongodb operation");

                await MigrateArticles(originalDbContext, targetMongoClient, logger).CAF();
                await MigrateTags(originalDbContext, targetMongoClient, logger).CAF();
                await MigrateUsers(originalDbContext, targetMongoClient, logger).CAF();
            }).Wait();
        }

        private static async Task MigrateArticles(Data originalDbContext, IMongoClient targetMongoClient,
            ILogger logger)
        {
            logger.LogCritical("Starting the article collection");
            var articleCollection = targetMongoClient.Article();

            await originalDbContext.Article.AsNoTracking().ForEachAsync(async (article) =>
            {
                string articleId = $"{article.CreatedYear.ToString()}-{article.TitleShrinked}";
                var targetArticle = await (await articleCollection.FindAsync(x => x.Id == articleId).CAF()).FirstOrDefaultAsync()
                    .CAF();

                if (targetArticle is null)
                {
                    article.Id = articleId;
                    await articleCollection.InsertOneAsync(article).CAF();
                    logger.LogCritical("Article with id: {id} was added to MongoDB", articleId);
                }
                else
                {
                    logger.LogCritical("Article with id: {id} already exists in MongoDB", articleId);
                }
            }).CAF();
            
            logger.LogCritical("Finished the article collection");
        }

        private static async Task MigrateTags(Data originalDbContext, IMongoClient targetMongoClient,
            ILogger logger)
        {
            logger.LogCritical("Starting the tag collection");
            var tagCollection = targetMongoClient.Tag();

            await originalDbContext.Tag.AsNoTracking().ForEachAsync(async (tag) =>
            {
                var targetTag = await (await tagCollection.FindAsync(x => x.Id == tag.Id).CAF()).FirstOrDefaultAsync()
                    .CAF();

                if (targetTag is null)
                {
                    await tagCollection.InsertOneAsync(tag).CAF();
                    logger.LogCritical("Tag with id: {id} was added to MongoDB", tag.Id);
                }
                else
                {
                    logger.LogCritical("Tag with id: {id} already exists in MongoDB", tag.Id);
                }
            }).CAF();
            
            logger.LogCritical("Finished the tag collection");
        }
        
        private static async Task MigrateUsers(Data originalDbContext, IMongoClient targetMongoClient,
            ILogger logger)
        {
            logger.LogCritical("Starting the user collection");
            var userCollection = targetMongoClient.User();

            await originalDbContext.User.AsNoTracking().ForEachAsync(async (user) =>
            {
                var targetUser = await (await userCollection.FindAsync(x => x.Id == user.Id).CAF()).FirstOrDefaultAsync()
                    .CAF();

                if (targetUser is null)
                {
                    await userCollection.InsertOneAsync(user).CAF();
                    logger.LogCritical("User with id: {id} was added to MongoDB", user.Id);
                }
                else
                {
                    logger.LogCritical("User with id: {id} already exists in MongoDB", user.Id);
                }
            }).CAF();
            
            logger.LogCritical("Finished the user collection");
        }
    }
}