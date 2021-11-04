using System.Text.Json.Serialization;

namespace Gem.Spaas.Shared.ProductionPlanComponent.Model
{
    public class Fuels
    {
        [JsonPropertyName("gas(euro/MWh)")]
        public double GasCostEuroMWh { get; set; }

        [JsonPropertyName("kerosine(euro/MWh)")]
        public double KerosineCostEuroMWh { get; set; }

        [JsonPropertyName("co2(euro/ton)")]
        public int Co2EuroTon { get; set; }

        [JsonPropertyName("wind(%)")]
        public int WindPercentage { get; set; }

        public override string ToString()
        {
            return $"{GasCostEuroMWh} {KerosineCostEuroMWh} {Co2EuroTon} {WindPercentage}";
        }
    }
}
