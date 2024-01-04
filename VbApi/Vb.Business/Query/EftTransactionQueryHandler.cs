// EftTransactionQueryHandler.cs

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;
using Vb.Schema;

public class EftTransactionQueryHandler :
    IRequestHandler<GetAllEftTransactionsQuery, ApiResponse<List<EftTransactionResponse>>>,
    IRequestHandler<GetEftTransactionByIdQuery, ApiResponse<EftTransactionResponse>>
{
    private readonly VbDbContext dbContext;
    private readonly IMapper mapper;

    public EftTransactionQueryHandler(VbDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<List<EftTransactionResponse>>> Handle(GetAllEftTransactionsQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var list = await dbContext.Set<EftTransaction>().ToListAsync(cancellationToken);
            var mappedList = mapper.Map<List<EftTransaction>, List<EftTransactionResponse>>(list);
            return new ApiResponse<List<EftTransactionResponse>>(mappedList);
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<EftTransactionResponse>>($"Error getting EftTransactions: {ex.Message}");
        }
    }

    public async Task<ApiResponse<EftTransactionResponse>> Handle(GetEftTransactionByIdQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            var entity = await dbContext.Set<EftTransaction>().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (entity == null)
            {
                return new ApiResponse<EftTransactionResponse>("Record not found");
            }

            var mapped = mapper.Map<EftTransaction, EftTransactionResponse>(entity);
            return new ApiResponse<EftTransactionResponse>(mapped);
        }
        catch (Exception ex)
        {
            return new ApiResponse<EftTransactionResponse>($"Error getting EftTransaction by ID: {ex.Message}");
        }
    }
}
