﻿using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Vb.Base.Response;
using Vb.Business.Cqrs;
using Vb.Data;
using Vb.Data.Entity;
using Vb.Schema;

namespace Vb.Business.Query
{
    public class ContactQueryHandler :
        IRequestHandler<GetAllContactsQuery, ApiResponse<List<ContactResponse>>>,
        IRequestHandler<GetContactByIdQuery, ApiResponse<ContactResponse>>
    {
        private readonly VbDbContext dbContext;
        private readonly IMapper mapper;

        public ContactQueryHandler(VbDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        public async Task<ApiResponse<List<ContactResponse>>> Handle(GetAllContactsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var list = await dbContext.Set<Contact>()
                    .Include(x => x.Customer)
                    .ToListAsync(cancellationToken);

                var mappedList = mapper.Map<List<Contact>, List<ContactResponse>>(list);
                return new ApiResponse<List<ContactResponse>>(mappedList);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ContactResponse>>($"Error getting contacts: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ContactResponse>> Handle(GetContactByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await dbContext.Set<Contact>()
                    .Include(x => x.Customer)
                    .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                if (entity == null)
                {
                    return new ApiResponse<ContactResponse>("Record not found");
                }

                var mapped = mapper.Map<Contact, ContactResponse>(entity);
                return new ApiResponse<ContactResponse>(mapped);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ContactResponse>($"Error getting contact by ID: {ex.Message}");
            }
        }
    }
}