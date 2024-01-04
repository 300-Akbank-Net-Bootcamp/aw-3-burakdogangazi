// AccountController.cs

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Schema;

namespace VbApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ApiResponse<List<AccountResponse>>> GetAll()
        {
            var operation = new GetAllAccountsQuery();
            return await _mediator.Send(operation);
        }

        [HttpGet("{id}")]
        public async Task<ApiResponse<AccountResponse>> GetById(int id)
        {
            var operation = new GetAccountByIdQuery(id);
            return await _mediator.Send(operation);
        }
        
        [HttpPost]
        public async Task<ApiResponse<AccountResponse>> Post([FromBody] AccountRequest account)
        {
            var operation = new CreateAccountCommand(account);
            return await _mediator.Send(operation);
        }

        [HttpPut("{id}")]
        public async Task<ApiResponse> Put(int id, [FromBody] AccountRequest account)
        {
            var operation = new UpdateAccountCommand(id, account);
            return await _mediator.Send(operation);
        }

        [HttpDelete("{id}")]
        public async Task<ApiResponse> Delete(int id)
        {
            var operation = new DeleteAccountCommand(id);
            return await _mediator.Send(operation);
        }
    }
}