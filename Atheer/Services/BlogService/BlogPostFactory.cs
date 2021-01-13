﻿using System;
using System.Text;
using Atheer.Controllers.ViewModels;
using Atheer.Models;
using AutoMapper;

namespace Atheer.Services.BlogService
{
    public sealed class BlogPostFactory
    {
        private readonly IMapper _mapper;

        public BlogPostFactory(IMapper mapper)
        {
            _mapper = mapper;
        }

        public BlogPost Create(ref BlogPostEditViewModel postViewModel, string userId)
        {
            var post = _mapper.Map<BlogPost>(postViewModel);

            post.AuthorId = userId;

            var date = DateTime.UtcNow;
            post.CreatedYear = date.Year;
            post.TitleShrinked = GetShrinkedTitle(postViewModel.Title);
            post.CreationDate = date.ToString();

            return post;
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