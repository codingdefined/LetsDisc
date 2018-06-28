using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using LetsDisc.Roles.Dto;
using LetsDisc.Users.Dto;

namespace LetsDisc.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedResultRequestDto, CreateUserDto, UserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();

        Task ChangeLanguage(ChangeUserLanguageDto input);
    }
}
