using Abp.Application.Services;
using Abp.Application.Services.Dto;
using LetsDisc.Tags.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LetsDisc.Tags
{
    public interface ITagAppService : IApplicationService
    {
        Task<PagedResultDto<TagDto>> GetTags(PagedResultRequestDto input);
    }
}
