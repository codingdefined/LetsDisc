using Abp.Application.Services;
using Abp.Application.Services.Dto;
using LetsDisc.MultiTenancy.Dto;

namespace LetsDisc.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}
