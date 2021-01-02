using System;
using System.Threading.Tasks;
using Atheer.Controllers.Dtos;
using Atheer.Models;
using Atheer.Repositories.Blog;
using AutoMapper;

namespace Atheer.Services.BlogService
{
    public class BlogPostService : IBlogPostService
    {
        private readonly BlogPostRepository _repository;
        private readonly IMapper _mapper;
        private readonly BlogPostFactory _factory;

        public BlogPostService(BlogPostRepository repository, IMapper mapper, BlogPostFactory factory)
        {
            _repository = repository;
            _mapper = mapper;
            _factory = factory;
        }

        public Task<BlogRepositoryBlogResponse> Get(int amount, PostsPaginationPrimaryKey paginationHeader = null)
        {
            return _repository.GetMany(amount, paginationHeader, false);
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

        public async Task AddPost(BlogPostEditDto postDto)
        { 
            var post = _factory.Create(ref postDto);
            string titleShrinked = post.TitleShrinked;
            
            // Check that no other post has same titleShrinked, else generate a new titleShrinked
            var key = new BlogPostPrimaryKey(post.CreatedYear, titleShrinked);
            while ((await GetSpecific(key).ConfigureAwait(false)) is not null)
            {
                titleShrinked = RandomiseExistingShrinkedTitle(ref titleShrinked);
                key.TitleShrinked = titleShrinked;
            }

            postDto.CreatedYear = post.CreatedYear;
            postDto.TitleShrinked = post.TitleShrinked = titleShrinked;

            await _repository.Add(post).ConfigureAwait(false);
        }

        private string RandomiseExistingShrinkedTitle(ref string existingTitleShrinked)
        {
            return $"{existingTitleShrinked}-";
        }

        public async Task Update(BlogPostEditDto postDto)
        {
            postDto.LastUpdatedDate = DateTime.UtcNow.ToString();

            await _repository.Update(postDto).ConfigureAwait(false);
        }
    }
}