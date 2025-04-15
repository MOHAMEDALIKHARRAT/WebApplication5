using WebApplication5.Models;

namespace WebApplication5.Repository
{
    public interface IOrderRepository
    {
        Task<Order> CreateAsync(Order order);
        Task<Order?> GetByIdAsync(int id);

    }
}
