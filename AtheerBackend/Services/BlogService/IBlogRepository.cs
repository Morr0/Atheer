﻿using System.Threading.Tasks;
using AtheerBackend.DTOs;
using AtheerCore.Models;

namespace AtheerBackend.Services.BlogService
{
    public interface IBlogRepository
    {
        Task<BlogRepositoryBlogResponse> Get(int amount, PostsPaginationPrimaryKey paginationHeader = null);

        Task<BlogRepositoryBlogResponse> GetByYear(int year, int amount, 
            PostsPaginationPrimaryKey paginationHeader = null);

        Task<BlogPost> Get(BlogPostPrimaryKey primaryKey);
        
        Task<BlogPost> Like(BlogPostPrimaryKey primaryKey);
        Task<BlogPost> Share(BlogPostPrimaryKey primaryKey);
    }
}