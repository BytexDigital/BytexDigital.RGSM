using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Scheduling;
using BytexDigital.RGSM.Node.Application.Exceptions;
using BytexDigital.RGSM.Node.Domain.Entities.Scheduling;
using BytexDigital.RGSM.Node.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core.Commands.Scheduling
{
    public class GetSchedulerQuery : IRequest<GetSchedulerQuery.Response>
    {
        public string ServerId { get; set; }
        public Func<IQueryable<SchedulerPlan>, IQueryable<SchedulerPlan>> Query { get; set; }

        public class Handler : IRequestHandler<GetSchedulerQuery, Response>
        {
            private readonly ServersService _serversService;
            private readonly SchedulersService _schedulersService;
            private readonly NodeDbContext _nodeDbContext;

            public Handler(ServersService serversService, SchedulersService schedulersService, NodeDbContext nodeDbContext)
            {
                _serversService = serversService;
                _schedulersService = schedulersService;
                _nodeDbContext = nodeDbContext;
            }

            public async Task<Response> Handle(GetSchedulerQuery request, CancellationToken cancellationToken)
            {
                var server = await _serversService.GetServer(request.ServerId).FirstOrDefaultAsync();

                if (server == null) throw new ServerNotFoundException();

                var query = _schedulersService.GetSchedulerPlan(server);

                if (request.Query != null) query = request.Query.Invoke(query);

                return new Response
                {
                    SchedulerPlan = await query.FirstAsync()
                };
            }
        }

        public class Response
        {
            public SchedulerPlan SchedulerPlan { get; set; }
        }
    }
}
