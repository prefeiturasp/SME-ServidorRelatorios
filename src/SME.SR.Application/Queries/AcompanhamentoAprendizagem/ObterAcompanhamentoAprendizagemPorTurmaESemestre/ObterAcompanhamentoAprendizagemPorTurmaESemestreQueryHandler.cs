using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAcompanhamentoAprendizagemPorTurmaESemestreQueryHandler : IRequestHandler<ObterAcompanhamentoAprendizagemPorTurmaESemestreQuery, IEnumerable<AcompanhamentoAprendizagemAlunoDto>>
    {
        private readonly IAcompanhamentoAprendizagemRepository acompanhamentoAprendizagemRepository;

        public ObterAcompanhamentoAprendizagemPorTurmaESemestreQueryHandler(IAcompanhamentoAprendizagemRepository acompanhamentoAprendizagemRepository)
        {
            this.acompanhamentoAprendizagemRepository = acompanhamentoAprendizagemRepository ?? throw new ArgumentNullException(nameof(acompanhamentoAprendizagemRepository));
        }

        public async Task<IEnumerable<AcompanhamentoAprendizagemAlunoDto>> Handle(ObterAcompanhamentoAprendizagemPorTurmaESemestreQuery request, CancellationToken cancellationToken)
            => await acompanhamentoAprendizagemRepository.ObterAcompanhamentoAprendizagemPorTurmaESemestre(request.TurmaId, request.AlunoCodigo, request.Semestre);        
    }
}
