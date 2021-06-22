using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAlunosPorTurmaAcompanhamentoApredizagemQueryHandler : IRequestHandler<ObterAlunosPorTurmaAcompanhamentoApredizagemQuery, IEnumerable<AlunoRetornoDto>>
    {
        private readonly IAlunoRepository alunoRepository;

        public ObterAlunosPorTurmaAcompanhamentoApredizagemQueryHandler(IAlunoRepository alunoRepository)
        {
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
        }

        public async Task<IEnumerable<AlunoRetornoDto>> Handle(ObterAlunosPorTurmaAcompanhamentoApredizagemQuery request, CancellationToken cancellationToken)
        {
            var alunos = await alunoRepository.ObterAlunosPorTurmaCodigoParaRelatorioAcompanhamentoAprendizagem(long.Parse(request.TurmaCodigo), request.AlunoCodigo, request.AnoLetivo);

            return alunos;
        }
    }
}
