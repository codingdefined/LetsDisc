using System.Threading.Tasks;
using Abp.Application.Services;
using LetsDisc.Authorization.Accounts.Dto;

namespace LetsDisc.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
