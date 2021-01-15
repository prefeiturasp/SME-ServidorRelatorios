using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAlunosResponsaveisPorTurmasCodigoRelatorioAdesaoQueryHandler : IRequestHandler<ObterAlunosResponsaveisPorTurmasCodigoRelatorioAdesaoQuery, IEnumerable<AlunoResponsavelAdesaoAEDto>>
    {
        private readonly IAlunoRepository alunoRepository;

        public ObterAlunosResponsaveisPorTurmasCodigoRelatorioAdesaoQueryHandler(IAlunoRepository alunoRepository)
        {
            this.alunoRepository = alunoRepository;
        }
        public async Task<IEnumerable<AlunoResponsavelAdesaoAEDto>> Handle(ObterAlunosResponsaveisPorTurmasCodigoRelatorioAdesaoQuery request, CancellationToken cancellationToken)
        {
            var anoAtual = DateTime.Now.Year;
            return await alunoRepository.ObterAlunosResponsaveisPorTurmasCodigoParaRelatorioAdesao(request.TurmasCodigo, anoAtual);
        }
    }
}
