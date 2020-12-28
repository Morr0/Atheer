using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AtheerBackend.DTOs;
using AtheerBackend.DTOs.BlogPost;
using AtheerBackend.Models;
using AtheerBackend.Repositories.Blog;
using AutoMapper;

namespace AtheerBackend.Services.BlogService
{
    public class BlogPostService : IBlogPostService
    {
        private readonly BlogPostRepository _repository;
        private readonly IMapper _mapper;

        public BlogPostService(BlogPostRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public Task<BlogRepositoryBlogResponse> Get(int amount, PostsPaginationPrimaryKey paginationHeader = null)
        {
            return _repository.GetMany(amount, paginationHeader, false);
        }

        public Task<IEnumerable<BareBlogPostReadDTO>> GetBare()
        {
            return _repository.GetManyBare();
        }

        public Task<BlogRepositoryBlogResponse> GetByYear(int year, int amount, 
            PostsPaginationPrimaryKey paginationHeader = null)
        {
            return _repository.GetMany(year, amount, paginationHeader, false);
        }

        public Task<BlogPost> GetSpecific(BlogPostPrimaryKey primaryKey)
        {
            return _repository.Get(primaryKey);
        }

        public Task<BlogPost> Like(BlogPostPrimaryKey primaryKey)
        {
            return UpdateRecord(primaryKey, UpdateBlogPostOperation.UpdateLikes);
        }

        public Task<BlogPost> Share(BlogPostPrimaryKey primaryKey)
        {
            return UpdateRecord(primaryKey, UpdateBlogPostOperation.UpdateShares);
        }

        private Task<BlogPost> UpdateRecord(BlogPostPrimaryKey primaryKey, UpdateBlogPostOperation operation)
        {
            string propertyToIncrement =
                operation == UpdateBlogPostOperation.UpdateLikes ? nameof(BlogPost.Likes) : nameof(BlogPost.Shares);
            string conditionProperty =
                operation == UpdateBlogPostOperation.UpdateLikes ? nameof(BlogPost.Likeable) : nameof(BlogPost.Shareable);

            return _repository.IncrementSpecificPropertyIf(primaryKey, propertyToIncrement, conditionProperty);
        }

        public Task Delete(BlogPostPrimaryKey key)
        {
            return _repository.Delete(key);
        }

        public Task<BlogPost> Update(BlogPostPrimaryKey key, BlogPost newPost)
        {
            return _repository.Update(key, newPost);
        }

        public async Task<BlogPost> AddPost(BlogPostEditDto postDto)
        {
            int createdYear = DateTime.UtcNow.Year;
            string titleShrinked = GetShrinkedTitle(postDto.Title);

            // Check that no other post has same titleShrinked, else generate a new titleShrinked
            var key = new BlogPostPrimaryKey(createdYear, titleShrinked);
            while ((await GetSpecific(key).ConfigureAwait(false)) is not null)
            {
                titleShrinked = RandomiseExistingShrinkedTitle(ref titleShrinked);
                key.TitleShrinked = titleShrinked;
            }

            postDto.CreatedYear = createdYear;
            postDto.TitleShrinked = titleShrinked;

            var post = _mapper.Map<BlogPost>(postDto);
            return await _repository.Add(post).ConfigureAwait(false);
        }

        private string RandomiseExistingShrinkedTitle(ref string existingTitleShrinked)
        {
            return $"{existingTitleShrinked}-";
        }

        private string GetShrinkedTitle(string title, string anotherChance = null)
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

        public async Task<BlogPost> UpdatePost(BlogPostEditDto post)
        {
            Console.WriteLine("Updating");
            // TODO update and update date
            return null;
        }
    }
}