using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Repositories;
using LetsDisc.PostDetails;
using LetsDisc.Tags.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LetsDisc.Tags
{
    public class TagAppService : ApplicationService, ITagAppService
    {
        private readonly IRepository<Tag> _tagRepository;

        public TagAppService(IRepository<Tag> tagRepository)
        {
            _tagRepository = tagRepository;
        }

        // Getting all questions on the Home Page
        public async Task<PagedResultDto<TagDto>> GetTags(PagedResultRequestDto input)
        {
            var tagCount = await _tagRepository.CountAsync();
            var tags = await _tagRepository
                                    .GetAll()
                                    .ToListAsync();

            return new PagedResultDto<TagDto>
            {
                TotalCount = tagCount,
                Items = tags.MapTo<List<TagDto>>()
            };
        }
    }
}
