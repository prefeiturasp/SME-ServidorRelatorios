using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SME.SR.Data.Interfaces;

namespace SME.SR.Application
{
    public class ObterRelatorioRecuperacaoParalelaAlunoSecaoQueryHandler : IRequestHandler<ObterRelatorioRecuperacaoParalelaAlunoSecaoQuery, IEnumerable<RelatorioRecuperacaoParalelaRetornoQueryDto>>
    {
        private readonly IRelatorioRecuperacaoParalelaRepository relatorioRecuperacaoParalelaRepository;

        public ObterRelatorioRecuperacaoParalelaAlunoSecaoQueryHandler(IRelatorioRecuperacaoParalelaRepository relatorioRecuperacaoParalelaRepository)
        {
            this.relatorioRecuperacaoParalelaRepository = relatorioRecuperacaoParalelaRepository ?? throw new ArgumentNullException(nameof(relatorioRecuperacaoParalelaRepository));
        }

        public async Task<IEnumerable<RelatorioRecuperacaoParalelaRetornoQueryDto>> Handle(ObterRelatorioRecuperacaoParalelaAlunoSecaoQuery request, CancellationToken cancellationToken)
        {
            return await relatorioRecuperacaoParalelaRepository.ObterDadosDeSecao(request.TurmaCodigo, request.AlunoCodigo, request.Semestre); ;
        }
    }
}
