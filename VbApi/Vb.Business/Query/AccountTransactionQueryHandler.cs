using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;
using Vb.Schema;

namespace Vb.Business.Query
{
    public class AccountTransactionQueryHandler :
        IRequestHandler<GetAllAccountTransactionsQuery, ApiResponse<List<AccountTransactionResponse>>>,
        IRequestHandler<GetAccountTransactionByIdQuery, ApiResponse<AccountTransactionResponse>>
    {
        private readonly VbDbContext dbContext;
        private readonly IMapper mapper;

        public AccountTransactionQueryHandler(VbDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<ApiResponse<List<AccountTransactionResponse>>> Handle(GetAllAccountTransactionsQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var list = await dbContext.Set<AccountTransaction>().ToListAsync(cancellationToken);
                var mappedList = mapper.Map<List<AccountTransaction>, List<AccountTransactionResponse>>(list);
                return new ApiResponse<List<AccountTransactionResponse>>(mappedList);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<AccountTransactionResponse>>($"Error getting account transactions: {ex.Message}");
            }
        }

        public async Task<ApiResponse<AccountTransactionResponse>> Handle(GetAccountTransactionByIdQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                var entity = await dbContext.Set<AccountTransaction>().FindAsync(request.Id);
                
                if (entity == null)
                {
                    return new ApiResponse<AccountTransactionResponse>("Record not found");
                }

                var mapped = mapper.Map<AccountTransaction, AccountTransactionResponse>(entity);
                return new ApiResponse<AccountTransactionResponse>(mapped);
            }
            catch (Exception ex)
            {
                return new ApiResponse<AccountTransactionResponse>($"Error getting account transaction by ID: {ex.Message}");
            }
        }
    }
}
