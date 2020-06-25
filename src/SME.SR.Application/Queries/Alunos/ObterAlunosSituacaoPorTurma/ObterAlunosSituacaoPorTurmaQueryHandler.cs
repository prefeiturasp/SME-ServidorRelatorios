using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAlunosSituacaoPorTurmaQueryHandler : IRequestHandler<ObterAlunosSituacaoPorTurmaQuery, IEnumerable<AlunoSituacaoDto>>
    {
        private ITurmaRepository turmaRepository;

        public ObterAlunosSituacaoPorTurmaQueryHandler(ITurmaRepository turmaRepository)
        {
            this.turmaRepository = turmaRepository ?? throw new ArgumentNullException(nameof(turmaRepository));
        }


        public async Task<IEnumerable<AlunoSituacaoDto>> Handle(ObterAlunosSituacaoPorTurmaQuery request, CancellationToken cancellationToken)
            => await turmaRepository.ObterDadosAlunosSituacao(request.TurmaCodigo);
    }
}
