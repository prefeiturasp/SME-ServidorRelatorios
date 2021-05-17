using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAcompanhamentoAprendizagemPorTurmaESemestreQueryHandler : IRequestHandler<ObterAcompanhamentoAprendizagemPorTurmaESemestreQuery, IEnumerable<AcompanhamentoAprendizagemAlunoRetornoDto>>
    {
        private readonly IAcompanhamentoAprendizagemRepository acompanhamentoAprendizagemRepository;

        public ObterAcompanhamentoAprendizagemPorTurmaESemestreQueryHandler(IAcompanhamentoAprendizagemRepository acompanhamentoAprendizagemRepository)
        {
            this.acompanhamentoAprendizagemRepository = acompanhamentoAprendizagemRepository ?? throw new ArgumentNullException(nameof(acompanhamentoAprendizagemRepository));
        }

        public async Task<IEnumerable<AcompanhamentoAprendizagemAlunoRetornoDto>> Handle(ObterAcompanhamentoAprendizagemPorTurmaESemestreQuery request, CancellationToken cancellationToken)
        {
            var acompanhamentoAluno = await acompanhamentoAprendizagemRepository.ObterAcompanhamentoAprendizagemPorTurmaESemestre(request.TurmaId, request.AlunoCodigo, request.Semestre);

            return acompanhamentoAluno;
        }
    }
}
