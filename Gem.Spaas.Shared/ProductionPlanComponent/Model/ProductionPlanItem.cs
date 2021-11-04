using System.Text.Json.Serialization;

namespace Gem.Spaas.Shared.ProductionPlanComponent.Model
{
    public class ProductionPlanItem
    {
     
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("p")]
        public double Power { get; set; }

        public override string ToString()
        {
            return $"{Name} {Power}";
        }
    }
}
