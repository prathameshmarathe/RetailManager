using System.Net.Http;
using System.Threading.Tasks;
using DesktopUI.Library.Models;
using DesktopUI.Models;

namespace DesktopUI.Library.Api
{
    public interface IAPIHelper
    {
        Task<AuthenticatedUser> Authenticate(string username, string password);
        Task GetLoggedInUserInfo(string token);
        HttpClient ApiClient { get; }
        void LogOffUser();
    }
}