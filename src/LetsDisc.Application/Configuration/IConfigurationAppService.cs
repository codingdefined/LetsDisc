using System.Threading.Tasks;
using LetsDisc.Configuration.Dto;

namespace LetsDisc.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}
