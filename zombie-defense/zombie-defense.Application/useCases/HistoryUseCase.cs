using zombie_defense.Domain.Entities;
using zombie_defense.Domain.Ports;

namespace zombie_defense.Application.useCases
{
    public class HistoryUseCase(ISimulationRepository simulationRepository)
    {
        public async Task<IEnumerable<Simulation>> ExecuteAsync()
        {
            return await simulationRepository.GetAllAsync();
        }
    }
}
