using CleanDeal.Model.Models;

namespace CleanDeal.Repositories.IRepositories
{
    public interface IUserDiscountRepository
    {
        Task AddAsync(UserServiceDiscount discount);
        Task<IEnumerable<UserServiceDiscount>> GetActiveByUserAsync(string userId);
    }
}