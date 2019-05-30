using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using LetsDisc.PostDetails;
using LetsDisc.Posts.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsDisc.Posts
{
    public class PostAppService : AsyncCrudAppService<Post, PostDto, int, PagedResultRequestDto, CreatePostDto, PostDto>, IPostAppService
    {
        private readonly IRepository<Post> _postRepository;

        public PostAppService(IRepository<Post> postRepository): base(postRepository)
        {
            _postRepository = postRepository;
        }

        public override async Task<PostDto> Create(CreatePostDto input)
        {
            CheckCreatePermission();

            var post = ObjectMapper.Map<Post>(input);
            await _postRepository.InsertAsync(post);
            CurrentUnitOfWork.SaveChanges();

            return MapToEntityDto(post);
        }

        public override async Task<PostDto> Update(PostDto input)
        {
            CheckUpdatePermission();

            var post = await _postRepository.GetAsync(input.Id);
            MapToEntity(input, post);
            await _postRepository.UpdateAsync(post);

            return await Get(input);
        }

        public override async Task Delete(EntityDto<int> input)
        {
            CheckDeletePermission();
            var post = await _postRepository.GetAsync(input.Id);
            await _postRepository.DeleteAsync(post);
        }

        protected override void MapToEntity(PostDto input, Post post)
        {
            ObjectMapper.Map(input, post);
        }

        protected override PostDto MapToEntityDto(Post post)
        {
            var postDto = base.MapToEntityDto(post);
            return postDto;
        }

        protected override IQueryable<Post> CreateFilteredQuery(PagedResultRequestDto input)
        {
            return base.CreateFilteredQuery(input);
        }

        protected override async Task<Post> GetEntityByIdAsync(int id)
        {
            var post = await _postRepository.FirstOrDefaultAsync(x => x.Id == id);

            if (post == null)
            {
                throw new EntityNotFoundException(typeof(Post), id);
            }

            return post;
        }

        // Getting all questions on the Home Page
        public async Task<PagedResultDto<PostDto>> GetQuestions(PagedResultRequestDto input)
        {
            var questionCount = await _postRepository.CountAsync(p => p.PostTypeId == 1);
            var questions = await _postRepository
                                .GetAll()
                                .Include(a => a.CreatorUser)
                                .Where(a => a.PostTypeId == 1)
                                .OrderByDescending(a => a.CreationTime)
                                .ToListAsync();

            return new PagedResultDto<PostDto>
            {
                TotalCount = questionCount,
                Items = questions.MapTo<List<PostDto>>()
            };
        }
    }
}
