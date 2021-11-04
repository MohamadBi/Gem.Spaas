using Gem.Spaas.Service.WebSockets;
using Gem.Spaas.Shared.ProductionPlanComponent.Dto;
using Gem.Spaas.Shared.ProductionPlanComponent.Model;
using MediatR;
using Microsoft.Extensions.Logging;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Gem.Spaas.Service.ProductionPlanComponent.Application.Commands.Create
{
    /// <summary>
    /// Class CreateProductionPlanCommandHandler.
    /// Implements the <see cref="MediatR.IRequestHandler{CreateProductionPlanCommand, IList{Gem.Spaas.Shared.ProductionPlanComponent.Dto.ProductionPlanItemDto}}" />
    /// </summary>
    /// <seealso cref="MediatR.IRequestHandler{CreateProductionPlanCommand, IList{Gem.Spaas.Shared.ProductionPlanComponent.Dto.ProductionPlanItemDto}}" />
    public class CreateProductionPlanCommandHandler : IRequestHandler<CreateProductionPlanCommand, IList<ProductionPlanItemDto>>
    {
        private readonly ILogger<CreateProductionPlanCommandHandler> _logger;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="CreateProductionPlanCommandHandler"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public CreateProductionPlanCommandHandler(ILogger<CreateProductionPlanCommandHandler> logger)
        {
            _logger = logger;                      
        }

        /// <summary>Handles a request</summary>
        /// <param name="request">The request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Response from the request</returns>
        /// <exception cref="System.ArgumentNullException">request</exception>
        /// <exception cref="System.Exception">You don't have enough plants to respond to the requested load, you still need {requiredPower - generatedPower} to generate</exception>
        public async Task<IList<ProductionPlanItemDto>> Handle(CreateProductionPlanCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request));
                }

                // Final production plan
                var result = new List<ProductionPlanItemDto>();

                // Generate the necessary power based on the requested demand                
                List<ProductionPlanItem> productionPlanItems = GeneratePower(request);

                // Final Result
                // Convert to dto
                result.AddRange(productionPlanItems.ToList()?.Select(x => ProductionPlanItemDto.Convert(x)));

                // Broadcast the result to connected websocket clients 
                SendWebSocketBroadcast(request, result);

                // Notify successful process
                _logger.LogInformation("Production plan successfully generated");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to generate a production plan: {ex.Message}");
                return null;
            }
        }

        /// <summary>Sends the web socket broadcast.</summary>
        /// <param name="request">The request.</param>
        /// <param name="result">The result.</param>
        private void SendWebSocketBroadcast(CreateProductionPlanCommand request, List<ProductionPlanItemDto> result)
        {
            // Prepare webSocket message
            var webSocketDto = new WebSocketMessageDto
            {
                Fuels = request.Fuels,
                ProductionPlanItemDtos = result,
                Load = request.Load,
                PowerPlants = request.PowerPlants,
            };

            BroadcastWebSocketMessage(webSocketDto);
        }

        /// <summary>
        /// Generates the power.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>List&lt;ProductionPlanItem&gt;.</returns>
        /// <exception cref="System.Exception">You don't have enough plants to respond to the requested load, you still need {request.Load - generatedPower} to generate</exception>
        private List<ProductionPlanItem> GeneratePower(CreateProductionPlanCommand request)
        {

            var productionPlanItems = new List<ProductionPlanItem>();
            List<PowerPlant> availablePlants = PreparePowerPlants(request);
            double requiredPower = request.Load;
            double generatedPower = 0;

            // Check if we have already one power plant that meets the requested load                
            PowerPlant onlyOnePlantRequired = !request.UseCo2 ? availablePlants.Find(p => requiredPower <= p.EffictivePowerMax && requiredPower > p.PowerMin && p.Efficiency > 0.8) :
                availablePlants.Find(p => requiredPower <= p.EffictivePowerMax && requiredPower > p.PowerMin && p.Efficiency > 0.8 && p.Co2Cost == 0);

            if (onlyOnePlantRequired != null && onlyOnePlantRequired.TotalFuelCost < (availablePlants.Sum(p => p.TotalFuelCost) - onlyOnePlantRequired.TotalFuelCost))
            {

                productionPlanItems.Add(new ProductionPlanItem
                {
                    Name = onlyOnePlantRequired.Name,
                    Power = requiredPower,
                });

                generatedPower += requiredPower;
                requiredPower = 0;

                onlyOnePlantRequired.IsUsed = true;
            }
            else
            {
                // Search all plants to aggregate requested load
                productionPlanItems = GenerateRequiredPower(requiredPower, ref generatedPower, availablePlants);
            }

            // Not enough power plants to meet the demand
            if (request.Load > generatedPower)
            {
                throw new Exception($"You don't have enough plants to respond to the requested load, you still need {request.Load - generatedPower} to generate");
            }
            
            // Add not used power plats to final result as there power = 0
            AddNotUsedPlant(request, productionPlanItems);

            return productionPlanItems;
        }

        /// <summary>
        /// Prepares the power plants.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>List&lt;PowerPlant&gt;.</returns>
        private List<PowerPlant> PreparePowerPlants(CreateProductionPlanCommand request)
        {
            // Exclude wind turbine if no wind available
            List<PowerPlant> availablePlants = request.Fuels.WindPercentage == 0 ? request.PowerPlants.Where(p => p.Type != FuelType.WindTurbine).ToList() : request.PowerPlants.ToList();

            // Prepare required values to generate the plan
            CalculateFuelCostAndEffectiveEnergy(availablePlants, request.Fuels);

            // Order available power plants based on efficiency => max power => cost
            availablePlants = !request.UseCo2 ? availablePlants.OrderByDescending(p => p.Efficiency).ThenByDescending(p => p.EffictivePowerMax).ThenBy(p => p.TotalFuelCost).ToList() :
                availablePlants.OrderBy(p => p.Co2Cost).ThenByDescending(p => p.Efficiency).ThenByDescending(p => p.EffictivePowerMax).ThenBy(p => p.TotalFuelCost).ToList();
            return availablePlants;
        }

        /// <summary>
        /// Generates the required power by using the necessary plants.
        /// </summary>
        /// <param name="requiredPower">The required power.</param>
        /// <param name="generatedPower">The generated power.</param>
        /// <param name="availablePlants">All plants.</param>
        /// <returns>List&lt;ProductionPlanItem&gt;.</returns>
        private List<ProductionPlanItem> GenerateRequiredPower(double requiredPower, ref double generatedPower, List<PowerPlant> availablePlants)
        {            
            var productionPlanItems = new List<ProductionPlanItem>();
            
            // Try to generate with power max
            productionPlanItems = GenerateByEffectivePowerMax(requiredPower, ref generatedPower, availablePlants);

            var availbleMaxPower = availablePlants.Sum(p => p.EffictivePowerMax);
            
            // If the power max does not meet the load, and we have plants available to generate the requested load, then we use power min
            if (requiredPower > generatedPower && availbleMaxPower > requiredPower)
            {
                generatedPower = 0;
                availablePlants.ForEach(p => p.IsUsed = false);    
                productionPlanItems = GenerateByPowerMin(requiredPower, ref generatedPower, availablePlants);
            }
            return productionPlanItems;
        }


        /// <summary>
        /// Generates the requested power by using plant minimum power first.
        /// </summary>
        /// <param name="requiredPower">The required power.</param>
        /// <param name="generatedPower">The generated power.</param>
        /// <param name="availablePlants">Available plants.</param>
        /// <returns>List&lt;ProductionPlanItem&gt;.</returns>
        private List<ProductionPlanItem> GenerateByPowerMin(double requiredPower, ref double generatedPower, List<PowerPlant> availablePlants)
        {
            var productionPlanItems = new List<ProductionPlanItem>();
            foreach (var plant in availablePlants)
            {
                if (requiredPower == 0) break;

                if (requiredPower <= plant.EffictivePowerMax)
                {
                    productionPlanItems.Add(new ProductionPlanItem
                    {
                        Name = plant.Name,
                        Power = requiredPower,
                    });

                    generatedPower += requiredPower;
                    requiredPower = 0;

                    plant.IsUsed = true;

                }
                else if (requiredPower > plant.PowerMin)
                {
                    productionPlanItems.Add(new ProductionPlanItem
                    {
                        Name = plant.Name,
                        Power = requiredPower - plant.PowerMin,
                    });

                    generatedPower += requiredPower - plant.PowerMin;
                    requiredPower -= requiredPower - plant.PowerMin;

                    plant.IsUsed = true;
                }
                else
                {
                    productionPlanItems.Add(new ProductionPlanItem
                    {
                        Name = plant.Name,
                        Power = requiredPower,
                    });

                    generatedPower += requiredPower;
                    requiredPower = 0;

                    plant.IsUsed = true;
                }

            }
            return productionPlanItems;
        }

        /// <summary>
        /// Generates the requested power by using effective power maximum first.
        /// </summary>
        /// <param name="requiredPower">The required power.</param>
        /// <param name="generatedPower">The generated power.</param>
        /// <param name="availablePlants">The available plants.</param>
        /// <returns>List&lt;ProductionPlanItem&gt;.</returns>
        private List<ProductionPlanItem> GenerateByEffectivePowerMax(double requiredPower, ref double generatedPower, List<PowerPlant> availablePlants)
        {
            var productionPlanItems = new List<ProductionPlanItem>();
            foreach (PowerPlant plant in availablePlants)
            {
                if (requiredPower == 0) break;

                if (requiredPower >= plant.EffictivePowerMax)
                {
                    productionPlanItems.Add(new ProductionPlanItem
                    {
                        Name = plant.Name,
                        Power = plant.EffictivePowerMax,
                    });

                    requiredPower -= plant.EffictivePowerMax;
                    generatedPower += plant.EffictivePowerMax;

                    plant.IsUsed = true;

                }
                else if (requiredPower > plant.PowerMin)
                {
                    productionPlanItems.Add(new ProductionPlanItem
                    {
                        Name = plant.Name,
                        Power = requiredPower,
                    });

                    generatedPower += requiredPower;
                    requiredPower = 0;

                    plant.IsUsed = true;
                }

            }
            return productionPlanItems;
        }

        /// <summary>
        ///   <para>
        ///     <br />
        ///   </para>
        ///   <para>Calculates the fuel cost and effective energy.
        /// </para>
        /// </summary>
        /// <param name="powerPlants">The power plants.</param>
        /// <param name="fuels">The fuels.</param>
        /// <exception cref="System.NotSupportedException">Power plant type {plant.Type} is not supported</exception>
        private void CalculateFuelCostAndEffectiveEnergy(IEnumerable<PowerPlant> powerPlants, Fuels fuels)
        {
            foreach (var plant in powerPlants)
            {
                switch (plant.Type)
                {
                    case FuelType.GasFired:
                        {
                            plant.TotalFuelCost = Math.Round(fuels.GasCostEuroMWh / plant.Efficiency, 1);
                            plant.EffictivePowerMax = plant.PowerMax;
                            plant.Co2Cost = Math.Round(plant.PowerMax * 0.3 * fuels.Co2EuroTon, 1);
                        }
                        break;
                    case FuelType.TurboJet:
                        {
                            plant.TotalFuelCost = Math.Round(fuels.KerosineCostEuroMWh / plant.Efficiency, 1);
                            plant.EffictivePowerMax = plant.PowerMax;
                        }
                        break;
                    case FuelType.WindTurbine:
                        {
                            plant.EffictivePowerMax = Math.Round(plant.PowerMax * fuels.WindPercentage * 0.01, 1);
                        }
                        break;
                    default: throw new NotSupportedException($"Power plant type {plant.Type} is not supported");

                }
            }

        }

        /// <summary>
        /// Adds the not used plants to final result.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="result">The result.</param>
        private static void AddNotUsedPlant(CreateProductionPlanCommand request, List<ProductionPlanItem> result)
        {
            result.AddRange(request.PowerPlants.Where(p => !p.IsUsed).Select(plant => new ProductionPlanItem
            {
                Name = plant.Name,
                Power = 0,
            }));
        }

        /// <summary>Broadcasts the web socket message.</summary>
        /// <param name="webSocketMessageDto">The web socket message dto.</param>
        /// <param name="logger">The logger.</param>
        private void BroadcastWebSocketMessage(WebSocketMessageDto webSocketMessageDto)
        {
            try
            {
                _logger.LogInformation("Broadcasting production plan generation info to all connected clients");
                var message = JsonSerializer.Serialize(webSocketMessageDto);
                ProductionPlanWebSocketServer.Broadcast(message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to broadcast websocket message: {ex.Message}");
            }
        }
    }
}
