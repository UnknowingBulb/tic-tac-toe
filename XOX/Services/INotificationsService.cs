using System.Threading.Tasks;

namespace XOX.Services
{
    public interface INotificationsService
    {
        Task SendNotificationAsync(string notification, string group);
    }
}
