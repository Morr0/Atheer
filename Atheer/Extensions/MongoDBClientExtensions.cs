﻿using Atheer.Models;
using MongoDB.Driver;

namespace Atheer.Extensions
{
    public static class MongoDBClientExtensions
    {
        public const string DatabaseName = "DB";
        
        public const string ArticleCollection = "Article";
        public const string ArticleSeriesCollection = "ArticleSeries";
        public const string TagCollection = "Tag";
        public const string UserCollection = "User";
        public const string UserLoginAttemptCollection = "UserLoginAttempt";
        public const string NavItemCollection = "NavItems";
        
        public static IMongoDatabase DB(this IMongoClient client)
        {
            return client.GetDatabase(DatabaseName);
        }
        
        public static IMongoCollection<Article> Article(this IMongoClient client)
        {
            return client.GetDatabase(DatabaseName).GetCollection<Article>(ArticleCollection);
        }
        
        public static IMongoCollection<ArticleSeries> ArticleSeries(this IMongoClient client)
        {
            return client.GetDatabase(DatabaseName).GetCollection<ArticleSeries>(ArticleSeriesCollection);
        }
        
        public static IMongoCollection<Atheer.Models.Tag> Tag(this IMongoClient client)
        {
            return client.GetDatabase(DatabaseName).GetCollection<Atheer.Models.Tag>(TagCollection);
        }
        
        public static IMongoCollection<User> User(this IMongoClient client)
        {
            return client.GetDatabase(DatabaseName).GetCollection<User>(UserCollection);
        }
        
        public static IMongoCollection<UserLoginAttempt> UserLoginAttempt(this IMongoClient client)
        {
            return client.GetDatabase(DatabaseName).GetCollection<UserLoginAttempt>(UserLoginAttemptCollection);
        }

        public static IMongoCollection<NavItem> NavItem(this IMongoClient client)
        {
            return client.GetDatabase(DatabaseName).GetCollection<NavItem>(NavItemCollection);
        }
    }
}