using System;
using System.Collections.Generic;
using System.Text;
using zombie_defense.Domain.Entities;

namespace zombie_defense.Domain.Ports
{
    public interface IZombieTypeRepository
    {
        Task<IEnumerable<ZombieType>> GetAllAsync();
    }
}
