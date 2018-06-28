using System.Threading.Tasks;
using Abp.Application.Services;
using LetsDisc.Sessions.Dto;

namespace LetsDisc.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
