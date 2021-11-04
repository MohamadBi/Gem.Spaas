using System.Text.Json.Serialization;

namespace Gem.Spaas.Shared.ProductionPlanComponent.Model
{
    public class PowerPlant
    {
        public PowerPlant()
        {

        }

        public PowerPlant(string name, FuelType type, double efficiency, double powerMin, double powerMax)
        {
            Name = name;
            Type = type;
            Efficiency = efficiency;
            PowerMin = powerMin;
            PowerMax = powerMax;                        
        }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FuelType Type { get; set; }

        [JsonPropertyName("efficiency")]
        public double Efficiency { get; set; }

        [JsonPropertyName("pmin")]
        public double PowerMin { get; set; }

        [JsonPropertyName("pmax")]
        public double PowerMax { get; set; }

        [JsonIgnore]
        public bool IsUsed { get; set; }

        [JsonIgnore]
        public double TotalFuelCost { get; set; }

        [JsonIgnore]
        public double Co2Cost { get; set; }

        [JsonIgnore]
        public double EffictivePowerMax { get; set; }

        public override string ToString()
        {
            return $"{Name} {Type} {Efficiency} {PowerMax} {PowerMin}";
        }
    }
}
