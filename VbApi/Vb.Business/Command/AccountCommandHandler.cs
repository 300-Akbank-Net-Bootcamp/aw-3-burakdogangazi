using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;
using Vb.Schema;

public class AccountCommandHandler :
    IRequestHandler<CreateAccountCommand, ApiResponse<AccountResponse>>,
    IRequestHandler<UpdateAccountCommand, ApiResponse>,
    IRequestHandler<DeleteAccountCommand, ApiResponse>
{
    private readonly VbDbContext dbContext;
    private readonly IMapper mapper;

    public AccountCommandHandler(VbDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }

    public async Task<ApiResponse<AccountResponse>> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var checkAccountNumber = await dbContext.Set<Account>().Where(x => x.AccountNumber == request.Model.AccountNumber)
                .FirstOrDefaultAsync(cancellationToken);

            if (checkAccountNumber != null)
            {
                return new ApiResponse<AccountResponse>($"{request.Model.AccountNumber} is used by another account.");
            }

            var entity = mapper.Map<AccountRequest, Account>(request.Model);
            entity.AccountNumber = new Random().Next(1000000, 9999999);

            var entityResult = await dbContext.AddAsync(entity, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            var mapped = mapper.Map<Account, AccountResponse>(entityResult.Entity);

            return new ApiResponse<AccountResponse>(mapped);
        }
        catch (Exception ex)
        {
            return new ApiResponse<AccountResponse>($"Error creating account: {ex.Message}");
        }
    }

    public async Task<ApiResponse> Handle(UpdateAccountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var fromdb = await dbContext.Set<Account>().Where(x => x.AccountNumber == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (fromdb == null)
            {
                return new ApiResponse("Record not found");
            }

            fromdb.AccountNumber = request.Model.AccountNumber;
            fromdb.Balance = request.Model.Balance;

            await dbContext.SaveChangesAsync(cancellationToken);

            return new ApiResponse();
        }
        catch (Exception ex)
        {
            return new ApiResponse($"Error updating account: {ex.Message}");
        }
    }

    public async Task<ApiResponse> Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var fromdb = await dbContext.Set<Account>().Where(x => x.AccountNumber == request.Id)
                .FirstOrDefaultAsync(cancellationToken);

            if (fromdb == null)
            {
                return new ApiResponse("Record not found");
            }

            fromdb.IsActive = false;

            await dbContext.SaveChangesAsync(cancellationToken);

            return new ApiResponse();
        }
        catch (Exception ex)
        {
            return new ApiResponse($"Error deleting account: {ex.Message}");
        }
    }
}
