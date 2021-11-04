using Gem.Spaas.Shared.ProductionPlanComponent.Model;
using System.Text.Json.Serialization;

namespace Gem.Spaas.Shared.ProductionPlanComponent.Dto
{
    public class ProductionPlanItemDto
    {

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("p")]
        public double Power { get; set; }

        public override string ToString()
        {
            return $"{Name} {Power}";
        }

        public static ProductionPlanItemDto Convert(ProductionPlanItem productionPlanItem)
        {
            return new ProductionPlanItemDto
            {
                Name = productionPlanItem.Name,
                Power = productionPlanItem.Power,
            };
        }
    }
}
