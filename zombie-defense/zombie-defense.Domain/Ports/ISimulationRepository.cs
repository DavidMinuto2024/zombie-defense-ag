using zombie_defense.Domain.Entities;

namespace zombie_defense.Domain.Ports
{
    public interface ISimulationRepository
    {
        Task<Simulation> SaveAsync(Simulation simulation);
        Task<IEnumerable<Simulation>> GetAllAsync();
    }
}
