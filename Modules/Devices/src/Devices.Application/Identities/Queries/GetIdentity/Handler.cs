﻿using Backbone.Modules.Devices.Application.Infrastructure.Persistence.Repository;
using Backbone.Modules.Devices.Domain.Entities.Identities;
using Enmeshed.BuildingBlocks.Application.Abstractions.Exceptions;
using MediatR;

namespace Backbone.Modules.Devices.Application.Identities.Queries.GetIdentity;
public class Handler : IRequestHandler<GetIdentityQuery, GetIdentityResponse>
{
    private readonly IIdentitiesRepository _identitiesRepository;

    public Handler(IIdentitiesRepository identitiesRepository)
    {
        _identitiesRepository = identitiesRepository;
    }

    public async Task<GetIdentityResponse> Handle(GetIdentityQuery request, CancellationToken cancellationToken)
    {
        var identity = await _identitiesRepository.FindByAddress(request.Address, cancellationToken) ?? throw new NotFoundException(nameof(Identity));

        return new GetIdentityResponse(identity);
    }
}
