using MediatR;
using SME.SR.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class GamesQueryHandler : IRequestHandler<GamesQuery, string>
    {
        private readonly IExemploRepository exemploRepository;

        public GamesQueryHandler(IExemploRepository exemploRepository)
        {
            this.exemploRepository = exemploRepository ?? throw new ArgumentNullException(nameof(exemploRepository));
        }
        public async Task<string> Handle(GamesQuery request, CancellationToken cancellationToken)
        {
            return await exemploRepository.ObterGames();
        }
    }
}
