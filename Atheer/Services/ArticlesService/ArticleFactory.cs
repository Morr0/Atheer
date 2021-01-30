﻿using System;
using System.Text;
using Atheer.Controllers.ViewModels;
using Atheer.Extensions;
using Atheer.Models;
using AutoMapper;

namespace Atheer.Services.ArticlesService
{
    public sealed class ArticleFactory
    {
        private readonly IMapper _mapper;

        public ArticleFactory(IMapper mapper)
        {
            _mapper = mapper;
        }

        public Article Create(ref ArticleEditViewModel articleViewModel, string userId)
        {
            var article = _mapper.Map<Article>(articleViewModel);

            article.AuthorId = userId;

            var date = DateTime.UtcNow;
            article.CreatedYear = date.Year;
            article.TitleShrinked = GetShrinkedTitle(articleViewModel.Title);
            article.CreationDate = date.GetString();

            return article;
        }

        public void SetUpdated(ref Article article)
        {
            article.LastUpdatedDate = DateTime.UtcNow.GetString();
        }

        private string GetShrinkedTitle(string title)
        {
            string[] splitTitle = title.Split();
            var sb = new StringBuilder(splitTitle.Length * 2);
            char separator = '-';
            for (var index = 0; index < splitTitle.Length; index++)
            {
                var t = splitTitle[index];
                sb.Append($"{t.ToLower()}");
                if ((index + 1) != splitTitle.Length) sb.Append(separator);
            }

            return sb.ToString();
        }
    }
}