using Gem.Spaas.Shared.ProductionPlanComponent.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Gem.Spaas.Shared.ProductionPlanComponent.Dto
{
    public class WebSocketMessageDto
    {
        [JsonPropertyName("load")]
        public double Load { get; set; }

        [JsonPropertyName("fuels")]
        public Fuels Fuels { get; set; }

        [JsonPropertyName("powerPlants")]
        public List<PowerPlant> PowerPlants { get; set; }
        
        [JsonPropertyName("result")]
        public IList<ProductionPlanItemDto> ProductionPlanItemDtos { get; set; }
    }
}
