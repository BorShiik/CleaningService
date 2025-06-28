using CleanDeal.Models;
using CleanDeal.Repositories;

namespace CleanDeal.Services.Loyalty
{
    public class LoyaltyService : ILoyaltyService
    {
        private readonly ILoyaltyRepository _repo;

        public LoyaltyService(ILoyaltyRepository repo)
        {
            _repo = repo;
        }

        public async Task AwardPointsAsync(string userId, int points, string description)
        {
            var tx = new LoyaltyTransaction
            {
                UserId = userId,
                Points = points,
                Description = description
            };
            await _repo.AddAsync(tx);
        }

        public async Task RedeemPointsAsync(string userId, int points, string description)
        {
            await AwardPointsAsync(userId, -points, description);
        }

        public Task<int> GetBalanceAsync(string userId) => _repo.GetBalanceAsync(userId);
        public Task<IEnumerable<LoyaltyTransaction>> GetHistoryAsync(string userId) => _repo.GetByUserAsync(userId);
    }
}
