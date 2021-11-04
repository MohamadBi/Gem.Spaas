using Gem.Spaas.Service.ProductionPlanComponent.Application.Commands.Create;
using Gem.Spaas.Shared.ProductionPlanComponent.Model;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Gem.Spaas.Xunit.Tests.ProductionPlanComponent.Application.Commands.Create
{
    public class CreateProductionPlanCommandTests

    {
        private readonly Mock<ILogger<CreateProductionPlanCommandHandler>> _logger;
        private readonly Fuels _generalFuelsData;
        public CreateProductionPlanCommandTests()
        {
            _logger = new Mock<ILogger<CreateProductionPlanCommandHandler>>();
            _generalFuelsData = new Fuels { Co2EuroTon = 20, KerosineCostEuroMWh = 50, GasCostEuroMWh = 15, WindPercentage = 50 };
        }

        [Fact]
        public async Task GeneratePower_ChallengeExample1()
        {
            // arrange
            Fuels fuelsData = new Fuels { Co2EuroTon = 0, KerosineCostEuroMWh = 50.8, GasCostEuroMWh = 13.4, WindPercentage = 60 };
            CreateProductionPlanCommand command = new CreateProductionPlanCommand
            {
                Load = 480,
                Fuels = fuelsData,
                PowerPlants = new List<PowerPlant>()
                {
                    new PowerPlant("gasfiredbig1", FuelType.GasFired, 0.53, 100, 460),
                    new PowerPlant("gasfiredbig2", FuelType.GasFired, 0.53, 100, 460),
                    new PowerPlant("gasfiredsomewhatsmaller", FuelType.GasFired, 0.37, 40, 210),
                    new PowerPlant("tj1", FuelType.TurboJet, 0.3, 0, 16),
                    new PowerPlant("windpark1", FuelType.WindTurbine, 1, 0, 150),
                    new PowerPlant("windpark2", FuelType.WindTurbine, 1, 0, 36)
                }
            };

            // act            
            var handler = new CreateProductionPlanCommandHandler(_logger.Object);
            var result = await handler.Handle(command, new CancellationToken(false));

            // assert
            Assert.Equal(480, result.Select(x => x.Power).Sum());
            Assert.Equal(90, result.First(x => x.Name == "windpark1").Power);
            Assert.Equal(21.6, result.First(x => x.Name == "windpark2").Power);
            Assert.Equal(368.4, result.First(x => x.Name == "gasfiredbig1").Power);
            Assert.Equal(0, result.First(x => x.Name == "gasfiredbig2").Power);
            Assert.Equal(0, result.First(x => x.Name == "gasfiredsomewhatsmaller").Power);
            Assert.Equal(0, result.First(x => x.Name == "tj1").Power);
        }

        [Fact]
        public async Task GeneratePower_ChallengeExample2()
        {
            // arrange
            Fuels fuelsData = new Fuels { Co2EuroTon = 0, KerosineCostEuroMWh = 50.8, GasCostEuroMWh = 13.4, WindPercentage = 0 };
            CreateProductionPlanCommand command = new CreateProductionPlanCommand
            {
                Load = 480,
                Fuels = fuelsData,
                PowerPlants = new List<PowerPlant>()
                {
                    new PowerPlant("gasfiredbig1", FuelType.GasFired, 0.53, 100, 460),
                    new PowerPlant("gasfiredbig2", FuelType.GasFired, 0.53, 100, 460),
                    new PowerPlant("gasfiredsomewhatsmaller", FuelType.GasFired, 0.37, 40, 210),
                    new PowerPlant("tj1", FuelType.TurboJet, 0.3, 0, 16),
                    new PowerPlant("windpark1", FuelType.WindTurbine, 1, 0, 150),
                    new PowerPlant("windpark2", FuelType.WindTurbine, 1, 0, 36)
                }
            };

            // act            
            var handler = new CreateProductionPlanCommandHandler(_logger.Object);
            var result = await handler.Handle(command, new CancellationToken(false));

            // assert
            Assert.Equal(480, result.Select(x => x.Power).Sum());
            Assert.Equal(0, result.First(x => x.Name == "windpark1").Power);
            Assert.Equal(0, result.First(x => x.Name == "windpark2").Power);
            Assert.Equal(380, result.First(x => x.Name == "gasfiredbig1").Power);
            Assert.Equal(100, result.First(x => x.Name == "gasfiredbig2").Power);
            Assert.Equal(0, result.First(x => x.Name == "gasfiredsomewhatsmaller").Power);
            Assert.Equal(0, result.First(x => x.Name == "tj1").Power);
        }

        [Fact]
        public async Task GeneratePower_ChallengeExample3()
        {
            // arrange            
            Fuels fuelsData = new Fuels { Co2EuroTon = 0, KerosineCostEuroMWh = 50.8, GasCostEuroMWh = 13.4, WindPercentage = 60 };
            CreateProductionPlanCommand command = new CreateProductionPlanCommand
            {
                Load = 910,
                Fuels = fuelsData,
                PowerPlants = new List<PowerPlant>()
                {
                    new PowerPlant("gasfiredbig1", FuelType.GasFired, 0.53, 100, 460),
                    new PowerPlant("gasfiredbig2", FuelType.GasFired, 0.53, 100, 460),
                    new PowerPlant("gasfiredsomewhatsmaller", FuelType.GasFired, 0.37, 40, 210),
                    new PowerPlant("tj1", FuelType.TurboJet, 0.3, 0, 16),
                    new PowerPlant("windpark1", FuelType.WindTurbine, 1, 0, 150),
                    new PowerPlant("windpark2", FuelType.WindTurbine, 1, 0, 36)
                }
            };

            // act            
            var handler = new CreateProductionPlanCommandHandler(_logger.Object);
            var result = await handler.Handle(command, new CancellationToken(false));

            // assert
            Assert.Equal(910, result.Select(x => x.Power).Sum());
            Assert.Equal(90, result.First(x => x.Name == "windpark1").Power);
            Assert.Equal(21.6, result.First(x => x.Name == "windpark2").Power);
            Assert.Equal(460, result.First(x => x.Name == "gasfiredbig1").Power);
            Assert.Equal(338.4, result.First(x => x.Name == "gasfiredbig2").Power);
            Assert.Equal(0, result.First(x => x.Name == "gasfiredsomewhatsmaller").Power);
            Assert.Equal(0, result.First(x => x.Name == "tj1").Power);
        }
        [Fact]
        public async Task GeneratePower_NotEnoughPlants()
        {
            // arrange + act
            CreateProductionPlanCommand command = new CreateProductionPlanCommand

            {
                Load = 750,
                Fuels = new Fuels { Co2EuroTon = 20, KerosineCostEuroMWh = 50, GasCostEuroMWh = 15, WindPercentage = 50 },
                PowerPlants = new List<PowerPlant>()
                {
                    new PowerPlant("GasStation1", FuelType.GasFired, 0.5, 50, 100),
                    new PowerPlant("GasStation2", FuelType.GasFired, 0.5, 50, 100)
                }
            };

            var handler = new CreateProductionPlanCommandHandler(_logger.Object);
            var result = await handler.Handle(command, new CancellationToken(false));
            // assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GeneratePower_TooMuchPower()
        {
            // arrange + act
            CreateProductionPlanCommand command = new CreateProductionPlanCommand
            {
                Load = 20,
                Fuels = _generalFuelsData,
                PowerPlants = new List<PowerPlant>()
                {
                    new PowerPlant("Gas1", FuelType.GasFired, 0.5, 50, 100),
                    new PowerPlant("Wind1", FuelType.WindTurbine, 1, 0, 50)
                }
            };

            var handler = new CreateProductionPlanCommandHandler(_logger.Object);
            var result = await handler.Handle(command, new CancellationToken(false));
            // assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GeneratePower_UseWind()
        {
            // arrange + act
            CreateProductionPlanCommand command = new CreateProductionPlanCommand
            {
                Load = 25,
                Fuels = _generalFuelsData,
                PowerPlants = new List<PowerPlant>()
                {
                    new PowerPlant("Gas1", FuelType.GasFired, 0.5, 10, 100),
                    new PowerPlant("Wind1", FuelType.WindTurbine, 1, 0, 50)
                }
            };

            // act
            var handler = new CreateProductionPlanCommandHandler(_logger.Object);
            var result = await handler.Handle(command, new CancellationToken(false));

            // assert
            Assert.Equal(25, result.First(x => x.Name == "Wind1").Power);
            Assert.Equal(0, result.First(x => x.Name == "Gas1").Power);
        }

        [Fact]
        public async Task GeneratePower_WindAndGas()
        {
            // arrange
            CreateProductionPlanCommand command = new CreateProductionPlanCommand
            {
                Load = 50,
                Fuels = _generalFuelsData,
                PowerPlants = new List<PowerPlant>()
                {
                    new PowerPlant("Gas1", FuelType.GasFired, 0.5, 10, 100),
                    new PowerPlant("Wind1", FuelType.WindTurbine, 1, 0, 50)
                }
            };

            // act            
            var handler = new CreateProductionPlanCommandHandler(_logger.Object);
            var result = await handler.Handle(command, new CancellationToken(false));

            // assert
            Assert.Equal(25, result.First(x => x.Name == "Wind1").Power);
            Assert.Equal(25, result.First(x => x.Name == "Gas1").Power);
        }

        [Fact]
        public async Task GeneratePower_UseBestGasEfficiency()
        {
            CreateProductionPlanCommand command = new CreateProductionPlanCommand
            {
                Load = 20,
                Fuels = _generalFuelsData,
                PowerPlants = new List<PowerPlant>()
                {    new PowerPlant("Gas1", FuelType.GasFired, 0.5, 10, 100),
                    new PowerPlant("Gas2", FuelType.GasFired, 0.6, 10, 100),
                    new PowerPlant("Gas3", FuelType.GasFired, 0.8, 10, 100),
                    new PowerPlant("Gas4", FuelType.GasFired, 0.3, 10, 100),
                    new PowerPlant("Gas5", FuelType.GasFired, 0.45, 10, 100)
                }
            };

            // act            
            var handler = new CreateProductionPlanCommandHandler(_logger.Object);
            var result = await handler.Handle(command, new CancellationToken(false));

            // assert
            Assert.Equal(20, result.First(x => x.Name == "Gas3").Power);
            Assert.Equal(0, result.Where(x => x.Name != "Gas3").Select(x => x.Power).Sum());
        }

        [Fact]
        public async Task GeneratePower_UseAllGas()
        {
            CreateProductionPlanCommand command = new CreateProductionPlanCommand
            {
                Load = 490,
                Fuels = _generalFuelsData,
                PowerPlants = new List<PowerPlant>()
                {
                    new PowerPlant("Gas1", FuelType.GasFired, 0.5, 10, 100),
                    new PowerPlant("Gas2", FuelType.GasFired, 0.6, 10, 100),
                    new PowerPlant("Gas3", FuelType.GasFired, 0.8, 10, 100),
                    new PowerPlant("Gas4", FuelType.GasFired, 0.3, 10, 100),
                    new PowerPlant("Gas5", FuelType.GasFired, 0.45, 10, 100)
                }
            };

            // act
            var handler = new CreateProductionPlanCommandHandler(_logger.Object);
            var result = await handler.Handle(command, new CancellationToken(false));

            // assert
            Assert.Equal(100, result.First(x => x.Name == "Gas1").Power);
            Assert.Equal(100, result.First(x => x.Name == "Gas2").Power);
            Assert.Equal(100, result.First(x => x.Name == "Gas3").Power);
            Assert.Equal(90, result.First(x => x.Name == "Gas4").Power);
            Assert.Equal(100, result.First(x => x.Name == "Gas5").Power);
        }

        [Fact]
        public async Task GeneratePower_GetBestGasEfficency()
        {
            // arrange
            CreateProductionPlanCommand command = new CreateProductionPlanCommand
            {
                Load = 125,
                Fuels = _generalFuelsData,
                PowerPlants = new List<PowerPlant>()
                {
                      new PowerPlant("Wind1", FuelType.WindTurbine, 1, 0, 50),
                    new PowerPlant("Gas1", FuelType.GasFired, 0.5, 110, 200),
                    new PowerPlant("Gas2", FuelType.GasFired, 0.8, 80, 150)
                }
            };

            // act
            var handler = new CreateProductionPlanCommandHandler(_logger.Object);
            var result = await handler.Handle(command, new CancellationToken(false));

            // assert
            Assert.Equal(100, result.First(x => x.Name == "Gas2").Power);
            Assert.Equal(0, result.First(x => x.Name == "Gas1").Power);
        }

        [Fact]
        public async Task GeneratePower_GetKerosinePlant()
        {
            // arrange
            CreateProductionPlanCommand command = new CreateProductionPlanCommand
            {
                Load = 100,
                Fuels = _generalFuelsData,
                PowerPlants = new List<PowerPlant>()
                {      new PowerPlant("Wind1", FuelType.WindTurbine, 1, 0, 150),
                    new PowerPlant("Gas1", FuelType.GasFired, 0.5, 100, 200),
                    new PowerPlant("Kerosine1", FuelType.TurboJet, 0.5, 0, 200)
                }
            };

            // act
            var handler = new CreateProductionPlanCommandHandler(_logger.Object);
            var result = await handler.Handle(command, new CancellationToken(false));

            // assert
            Assert.Equal(0, result.First(x => x.Name == "Gas1").Power);
            Assert.Equal(25, result.First(x => x.Name == "Kerosine1").Power);
        }

        [Fact]
        public async Task GeneratePower_UseCo2()
        {
            // arrange
            CreateProductionPlanCommand command = new CreateProductionPlanCommand
            {
                Load = 150,
                Fuels = _generalFuelsData,
                PowerPlants = new List<PowerPlant>()
                    {
                        new PowerPlant("Gas1", FuelType.GasFired, 0.9, 100, 200),
                        new PowerPlant("Kerosine1", FuelType.TurboJet, 0.9, 0, 200)
                    }
            };

            // act                
            var handler = new CreateProductionPlanCommandHandler(_logger.Object);
            var resultNoCO2 = await handler.Handle(command, new CancellationToken(false));

            CreateProductionPlanCommand commandCo2 = new CreateProductionPlanCommand
            {
                Load = 150,
                UseCo2 = true,
                Fuels = _generalFuelsData,
                PowerPlants = new List<PowerPlant>()
                    {
                        new PowerPlant("Gas1", FuelType.GasFired, 0.9, 100, 200),
                        new PowerPlant("Kerosine1", FuelType.TurboJet, 0.9, 0, 200)
                    }
            };

            // act                
            var handlerCo2 = new CreateProductionPlanCommandHandler(_logger.Object);
            var resultCo2 = await handler.Handle(commandCo2, new CancellationToken(false));

            // assert
            Assert.Equal(150, resultNoCO2.First(x => x.Name == "Gas1").Power);
            Assert.Equal(150, resultCo2.First(x => x.Name == "Kerosine1").Power);
        }

        [Fact]
        public async Task GeneratePower_UseBestAvailableGasPlants()
        {
            // arrange
            Fuels fuelsData = new Fuels { Co2EuroTon = 0, KerosineCostEuroMWh = 50.8, GasCostEuroMWh = 20, WindPercentage = 100 };

            CreateProductionPlanCommand command = new CreateProductionPlanCommand
            {
                Load = 60,
                Fuels = fuelsData,
                PowerPlants = new List<PowerPlant>()
                {
                    new PowerPlant("windpark1", FuelType.WindTurbine, 1, 0, 20),
                    new PowerPlant("gasfired", FuelType.GasFired, 0.9, 50, 100),
                    new PowerPlant("gasfiredinefficient", FuelType.GasFired, 0.1, 0, 100)
                }
            };

            // act            
            var handler = new CreateProductionPlanCommandHandler(_logger.Object);
            var result = await handler.Handle(command, new CancellationToken(false));

            // assert
            Assert.Equal(60, result.Select(x => x.Power).Sum());
            Assert.Equal(0, result.First(x => x.Name == "windpark1").Power);
            Assert.Equal(60, result.First(x => x.Name == "gasfired").Power);
            Assert.Equal(0, result.First(x => x.Name == "gasfiredinefficient").Power);
        }

        [Fact]
        public async Task GeneratePower_UseBestAvailablMatch()
        {
            // arrange            
            Fuels fuelsData = new Fuels { Co2EuroTon = 0, KerosineCostEuroMWh = 50.8, GasCostEuroMWh = 20, WindPercentage = 100 };

            CreateProductionPlanCommand command = new CreateProductionPlanCommand
            {
                Load = 80,
                Fuels = fuelsData,
                PowerPlants = new List<PowerPlant>()
                {
                    new PowerPlant("windpark1", FuelType.WindTurbine, 1, 0, 60),
                    new PowerPlant("gasfired", FuelType.GasFired, 0.9, 50, 100),
                    new PowerPlant("gasfiredinefficient", FuelType.GasFired, 0.1, 0, 200)
                }
            };

            // act                   
            var handler = new CreateProductionPlanCommandHandler(_logger.Object);
            var result = await handler.Handle(command, new CancellationToken(false));

            // assert
            Assert.Equal(80, result.Select(x => x.Power).Sum());
            Assert.Equal(0, result.First(x => x.Name == "windpark1").Power);
            Assert.Equal(80, result.First(x => x.Name == "gasfired").Power);
            Assert.Equal(0, result.First(x => x.Name == "gasfiredinefficient").Power);
        }

     

        [Fact]
        public async Task GeneratePower_BasicTest()
        {
            var command = new CreateProductionPlanCommand()
            {
                Load = 150,
                Fuels = new Fuels
                {
                    Co2EuroTon = 5,
                    GasCostEuroMWh = 15,
                    WindPercentage = 10,
                    KerosineCostEuroMWh = 12
                },

                PowerPlants = new List<PowerPlant>
                {
                    new PowerPlant
                    {
                        Efficiency = 0.5,
                        Name = "gasfiredbig2",
                        Type =  FuelType.GasFired,
                        PowerMax = 400,
                        PowerMin = 100
                    }
                }

            };

            var handler = new CreateProductionPlanCommandHandler(_logger.Object);
            var result = await handler.Handle(command, new CancellationToken(false));

            Assert.Equal("gasfiredbig2", result[0].Name);
            Assert.Equal(150, result[0].Power);
        }
       
    }
}