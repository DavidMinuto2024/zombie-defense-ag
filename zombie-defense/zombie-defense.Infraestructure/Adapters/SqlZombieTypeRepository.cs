using Microsoft.EntityFrameworkCore;
using zombie_defense.Domain.Entities;
using zombie_defense.Domain.Ports;
using zombie_defense.Infraestructure.Persistence;

namespace zombie_defense.Infraestructure.Adapters
{
    public class SqlZombieTypeRepository(AppDbContext dbContext) : IZombieTypeRepository
    {
        public async Task<IEnumerable<ZombieType>> GetAllAsync()
        {
            return await dbContext.ZombieTypes.ToListAsync();
        }
    }
}
