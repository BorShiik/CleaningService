using CleanDeal.Models;

namespace CleanDeal.Repositories
{
    public interface IUserDiscountRepository
    {
        Task AddAsync(UserServiceDiscount discount);
        Task<IEnumerable<UserServiceDiscount>> GetActiveByUserAsync(string userId);
    }
}