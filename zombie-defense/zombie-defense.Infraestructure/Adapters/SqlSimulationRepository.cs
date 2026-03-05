using zombie_defense.Domain.Entities;
using zombie_defense.Domain.Ports;
using zombie_defense.Infraestructure.Persistence;

namespace zombie_defense.Infraestructure.Adapters
{
    public class SqlSimulationRepository(AppDbContext context) : ISimulationRepository
    {
        public async Task<Simulation> SaveAsync(Simulation simulation)
        {
            context.Simulations.Add(simulation);
            await context.SaveChangesAsync();
            return simulation;
        }
    }
}
