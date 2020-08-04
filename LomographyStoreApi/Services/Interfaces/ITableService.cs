using System.Collections.Generic;
using System.Threading.Tasks;
using LomographyStoreApi.Models;

namespace LomographyStoreApi.Services.Interfaces
{
    public interface ITableService
    {
        Task<List<OrderHistoryItem>> GetOrderHistoryAsync();
    }
}