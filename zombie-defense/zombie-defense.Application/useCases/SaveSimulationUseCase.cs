using System;
using System.Collections.Generic;
using System.Text;
using zombie_defense.Domain.Entities;
using zombie_defense.Domain.Ports;

namespace zombie_defense.Application.useCases
{
    public class SaveSimulationUseCase(ISimulationRepository simulationRepository)
    {
        public async Task<Simulation> ExecuteAsync(Simulation simulation)
        {
            return await simulationRepository.SaveAsync(simulation);
        }
    }
}
