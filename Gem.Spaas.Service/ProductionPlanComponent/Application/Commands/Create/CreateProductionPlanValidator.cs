using FluentValidation;

namespace Gem.Spaas.Service.ProductionPlanComponent.Application.Commands.Create
{
    public class CreateProductionPlanValidator : AbstractValidator<CreateProductionPlanCommand>
    {
        public CreateProductionPlanValidator()
        {
            RuleFor(x => x.Load).GreaterThan(0);
            RuleFor(x => x.Fuels).NotNull();
            RuleFor(x => x.PowerPlants).NotNull();

        }
    }
}
