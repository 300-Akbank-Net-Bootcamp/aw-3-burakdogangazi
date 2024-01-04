// EftTransactionController.cs

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Schema;

namespace VbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EftTransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EftTransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ApiResponse<List<EftTransactionResponse>>> GetAll()
        {
            var operation = new GetAllEftTransactionsQuery();
            return await _mediator.Send(operation);
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse<EftTransactionResponse>> GetById(int id)
        {
            var operation = new GetEftTransactionByIdQuery(id);
            return await _mediator.Send(operation);
        }

        [HttpPost]
        public async Task<ApiResponse<EftTransactionResponse>> Post([FromBody] EftTransactionRequest eftTransaction)
        {
            var operation = new CreateEftTransactionCommand(eftTransaction);
            return await _mediator.Send(operation);
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse> Put(int id, [FromBody] EftTransactionRequest eftTransaction)
        {
            var operation = new UpdateEftTransactionCommand(id, eftTransaction);
            return await _mediator.Send(operation);
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse> Delete(int id)
        {
            var operation = new DeleteEftTransactionCommand(id);
            return await _mediator.Send(operation);
        }
    }
}