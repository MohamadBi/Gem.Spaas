using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Gem.Spaas.Shared.ProductionPlanComponent.Model
{
    public enum FuelType
    {
        [EnumMember(Value = "gasfired")]
        GasFired,

        [EnumMember(Value = "turbojet")]
        TurboJet,

        [EnumMember(Value = "windturbine")]
        WindTurbine,
    }
}
