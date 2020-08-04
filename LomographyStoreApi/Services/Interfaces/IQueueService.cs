using System.Threading.Tasks;
using LomographyStoreApi.Models;

namespace LomographyStoreApi.Services.Interfaces
{
    public interface IQueueService
    {
         Task SendMessageAsync(Order item);
    }
}