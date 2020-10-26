using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRelatorioSondagemPortuguesCapLeituraPorTurmaQueryHandler : IRequestHandler<ObterRelatorioSondagemPortuguesCapLeituraPorTurmaQuery, RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaDto>
    {
        private readonly IRelatorioSondagemPortuguesPorTurmaRepository relatorioSondagemPortuguesPorTurmaRepository;
        private readonly IPerguntasAutoralRepository perguntasAutoralRepository;
        private readonly IMediator mediator;

        public ObterRelatorioSondagemPortuguesCapLeituraPorTurmaQueryHandler(
            IRelatorioSondagemPortuguesPorTurmaRepository relatorioSondagemPortuguesPorTurmaRepository,
            IMediator mediator, IPerguntasAutoralRepository perguntasAutoralRepository)
        {
            this.relatorioSondagemPortuguesPorTurmaRepository = relatorioSondagemPortuguesPorTurmaRepository ?? throw new ArgumentNullException(nameof(relatorioSondagemPortuguesPorTurmaRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.perguntasAutoralRepository = perguntasAutoralRepository ?? throw new ArgumentNullException(nameof(perguntasAutoralRepository));
        }

        public async Task<RelatorioSondagemPortuguesCapacidadeLeituraPorTurmaDto> Handle(ObterRelatorioSondagemPortuguesCapLeituraPorTurmaQuery request, CancellationToken cancellationToken)
        {
            var dataDoPeriodo = await mediator.Send(new ObterDataPeriodoFimSondagemPorSemestreAnoLetivoQuery(request.Semestre, request.AnoLetivo));

            var alunosDaTurma = await mediator.Send(new ObterAlunosPorTurmaDataSituacaoMatriculaQuery(request.TurmaCodigo, dataDoPeriodo));
            if (alunosDaTurma == null || !alunosDaTurma.Any())
                throw new NegocioException("Não foi possível localizar os alunos da turma.");

            var perguntas =  await perguntasAutoralRepository.ObterPerguntasPorGrupo(GrupoSondagemEnum.CapacidadeLeitura, ComponenteCurricularSondagemEnum.Portugues);

            var dados = await relatorioSondagemPortuguesPorTurmaRepository.ObterPorFiltros(GrupoSondagemEnum.CapacidadeLeitura.Name(), ComponenteCurricularSondagemEnum.Portugues.Name(),0, request.AnoLetivo, request.TurmaCodigo);
        }

        
    }
}
