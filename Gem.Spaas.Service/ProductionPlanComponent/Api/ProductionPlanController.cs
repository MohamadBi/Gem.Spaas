using Gem.Spaas.Service.ProductionPlanComponent.Application.Commands.Create;
using Gem.Spaas.Shared.ProductionPlanComponent.Dto;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Gem.Spaas.Service.ProductionPlanComponent.Api
{
    [Route("/productionplan")]
    [ApiController]
    public class ProductionPlanController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductionPlanController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost]
        [Route("")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IList<ProductionPlanItemDto>>> Create([FromBody] CreateProductionPlanCommand creatProductionPlanCommand)
        {
            return Ok(await _mediator.Send(creatProductionPlanCommand));
        }

    }

}
