using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BytexDigital.RGSM.Node.Application.Core.Scheduling;
using BytexDigital.RGSM.Node.Domain.Entities.Scheduling;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace BytexDigital.RGSM.Node.Application.Core.Commands.Scheduling
{
    public class GetSchedulersQuery : IRequest<GetSchedulersQuery.Response>
    {
        public Func<IQueryable<SchedulerPlan>, IQueryable<SchedulerPlan>> Query { get; set; }

        public class Handler : IRequestHandler<GetSchedulersQuery, Response>
        {
            private readonly SchedulersService _schedulersService;

            public Handler(SchedulersService schedulersService)
            {
                _schedulersService = schedulersService;
            }

            public async Task<Response> Handle(GetSchedulersQuery request, CancellationToken cancellationToken)
            {
                var query = _schedulersService.GetSchedulerPlans();

                if (request.Query != null) query = request.Query.Invoke(query);

                return new Response
                {
                    SchedulerPlans = await query.ToListAsync()
                };
            }
        }

        public class Response
        {
            public List<SchedulerPlan> SchedulerPlans { get; set; }
        }
    }
}
