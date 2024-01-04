// EftTransactionCommandHandler.cs

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;
using Vb.Schema;

public class EftTransactionCommandHandler :
    IRequestHandler<CreateEftTransactionCommand, ApiResponse<EftTransactionResponse>>,
    IRequestHandler<UpdateEftTransactionCommand, ApiResponse>,
    IRequestHandler<DeleteEftTransactionCommand, ApiResponse>
{
    private readonly VbDbContext dbContext;
    private readonly IMapper mapper;

    public EftTransactionCommandHandler(VbDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<EftTransactionResponse>> Handle(CreateEftTransactionCommand request, CancellationToken cancellationToken)
    {
        try
        {

            var entity = mapper.Map<EftTransactionRequest, EftTransaction>(request.Model);
            await dbContext.AddAsync(entity, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            var mapped = mapper.Map<EftTransaction, EftTransactionResponse>(entity);
            return new ApiResponse<EftTransactionResponse>(mapped);
        }
        catch (Exception ex)
        {
            return new ApiResponse<EftTransactionResponse>($"Error creating EftTransaction: {ex.Message}");
        }
    }

    public async Task<ApiResponse> Handle(UpdateEftTransactionCommand request, CancellationToken cancellationToken)
    {
        try
        {

            var fromDb = await dbContext.Set<EftTransaction>().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (fromDb == null)
            {
                return new ApiResponse("Record not found");
            }

            fromDb.ReferenceNumber = request.Model.ReferenceNumber;
            fromDb.TransactionDate = request.Model.TransactionDate;
            fromDb.Amount = request.Model.Amount;
            fromDb.Description = request.Model.Description;
            fromDb.SenderAccount = request.Model.SenderAccount;
            fromDb.SenderIban = request.Model.SenderIban;
            fromDb.SenderName = request.Model.SenderName;

            await dbContext.SaveChangesAsync(cancellationToken);

            return new ApiResponse();
        }
        catch (Exception ex)
        {
            return new ApiResponse($"Error updating EftTransaction: {ex.Message}");
        }
    }

    public async Task<ApiResponse> Handle(DeleteEftTransactionCommand request, CancellationToken cancellationToken)
    {
        try
        {

            var fromDb = await dbContext.Set<EftTransaction>().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (fromDb == null)
            {
                return new ApiResponse("Record not found");
            }

            dbContext.Remove(fromDb);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new ApiResponse();
        }
        catch (Exception ex)
        {
            return new ApiResponse($"Error deleting EftTransaction: {ex.Message}");
        }
    }
}
