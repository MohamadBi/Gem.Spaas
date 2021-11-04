using Gem.Spaas.Shared.ProductionPlanComponent.Dto;
using Gem.Spaas.Shared.ProductionPlanComponent.Model;
using MediatR;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Gem.Spaas.Service.ProductionPlanComponent.Application.Commands.Create
{
    public class CreateProductionPlanCommand : IRequest<IList<ProductionPlanItemDto>>
    {        
        public double Load { get; set; }
     
        public Fuels Fuels { get; set; }
      
        public List<PowerPlant> PowerPlants { get; set; }

        [JsonIgnore]
        public bool UseCo2 { get; set; }
    }
}
