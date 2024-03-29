﻿using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;
using Vb.Schema;

namespace Vb.Business.Command
{
    public class AccountTransactionCommandHandler :
        IRequestHandler<InsertAccountTransactionCommand, ApiResponse<AccountTransactionResponse>>,
        IRequestHandler<UpdateAccountTransactionCommand, ApiResponse>,
        IRequestHandler<DeleteAccountTransactionCommand, ApiResponse>
    {
        private readonly VbDbContext dbContext;
        private readonly IMapper mapper;

        public AccountTransactionCommandHandler(VbDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<ApiResponse<AccountTransactionResponse>> Handle(InsertAccountTransactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = mapper.Map<AccountTransactionRequest, AccountTransaction>(request.Model);

                var entityResult = await dbContext.AddAsync(entity, cancellationToken);
                await dbContext.SaveChangesAsync(cancellationToken);

                var mapped = mapper.Map<AccountTransaction, AccountTransactionResponse>(entityResult.Entity);

                return new ApiResponse<AccountTransactionResponse>(mapped);
            }
            catch (Exception ex)
            {
                return new ApiResponse<AccountTransactionResponse>($"Error inserting account transaction: {ex.Message}");
            }
        }

        public async Task<ApiResponse> Handle(UpdateAccountTransactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var fromdb = await dbContext.Set<AccountTransaction>().Where(x => x.Id == request.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (fromdb == null)
                {
                    return new ApiResponse("Record not found");
                }

                fromdb.AccountId = request.Model.AccountId;
                fromdb.ReferenceNumber = request.Model.ReferenceNumber;
                fromdb.TransactionDate = request.Model.TransactionDate;
                fromdb.Amount = request.Model.Amount;
                fromdb.Description = request.Model.Description;
                fromdb.TransferType = request.Model.TransferType;

                await dbContext.SaveChangesAsync(cancellationToken);

                return new ApiResponse();
            }
            catch (Exception ex)
            {
                return new ApiResponse($"Error updating account transaction: {ex.Message}");
            }
        }

        public async Task<ApiResponse> Handle(DeleteAccountTransactionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var fromdb = await dbContext.Set<AccountTransaction>().Where(x => x.Id == request.Id)
                    .FirstOrDefaultAsync(cancellationToken);

                if (fromdb == null)
                {
                    return new ApiResponse("Record not found");
                }

                dbContext.Set<AccountTransaction>().Remove(fromdb);
                await dbContext.SaveChangesAsync(cancellationToken);

                return new ApiResponse();
            }
            catch (Exception ex)
            {
                return new ApiResponse($"Error deleting account transaction: {ex.Message}");
            }
        }
    }
}
