﻿using System.Threading.Tasks;
using Atheer.Controllers.ViewModels;
using Atheer.Models;

namespace Atheer.Services.ArticlesService
{
    public interface IArticleService
    {
        Task<bool> Exists(ArticlePrimaryKey key, string userId = null);
        Task<ArticleViewModel> Get(ArticlePrimaryKey primaryKey, string viewerUserId = null);
        Task<ArticlesResponse> Get(int amount, int page, int createdYear = 0, string tagId = null,
            string viewerUserId = null, string specificUserId = null);
        
        Task Like(ArticlePrimaryKey primaryKey);
        Task Share(ArticlePrimaryKey primaryKey);

        Task Delete(ArticlePrimaryKey key);
        Task Add(ArticleEditViewModel articleEditViewModel, string userId);
        Task Update(ArticleEditViewModel article);

        Task<bool> AuthorizedFor(ArticlePrimaryKey key, string userId);
    }
}
