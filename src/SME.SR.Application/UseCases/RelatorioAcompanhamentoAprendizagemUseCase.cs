using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioAcompanhamentoAprendizagemUseCase : IRelatorioAcompanhamentoAprendizagemUseCase
    {
        private readonly IMediator mediator;

        public RelatorioAcompanhamentoAprendizagemUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto filtro)
        {

            var parametros = filtro.ObterObjetoFiltro<FiltroRelatorioAcompanhamentoAprendizagemDto>();

            var turma = await mediator.Send(new ObterComDreUePorTurmaIdQuery(parametros.TurmaId));

            if (turma == null)
                throw new NegocioException("Turma não encontrada");

            var turmaCodigo = new string[1] { turma.Codigo };
            var professores = await mediator.Send(new ObterProfessorTitularComponenteCurricularPorTurmaQuery(turmaCodigo));

            var alunosEol = await mediator.Send(new ObterAlunosPorTurmaAcompanhamentoApredizagemQuery(turma.Codigo, parametros.AlunoCodigo, turma.AnoLetivo));
            if (alunosEol == null || !alunosEol.Any())
                throw new NegocioException("Nenhuma informação para os filtros informados.");

            var acompanhmentosAlunos = await mediator.Send(new ObterAcompanhamentoAprendizagemPorTurmaESemestreQuery(parametros.TurmaId, parametros.AlunoCodigo.ToString(), parametros.Semestre));

            var bimestres = ObterBimestresPorSemestre(parametros.Semestre);

            var tipoCalendarioId = await mediator.Send(new ObterIdTipoCalendarioPorAnoLetivoEModalidadeQuery(turma.AnoLetivo, turma.ModalidadeTipoCalendario, turma.Semestre));

            var periodoInicioFim = await ObterInicioFimPeriodo(tipoCalendarioId, bimestres, parametros.Semestre);

            var quantidadeAulasDadas = await mediator.Send(new ObterQuantiaddeAulasDadasPorTurmaEBimestreQuery(turma.Codigo, tipoCalendarioId, bimestres));

            var frequenciaAlunos = await mediator.Send(new ObterFrequenciaGeralAlunosPorTurmaEBimestreQuery(parametros.TurmaId, parametros.AlunoCodigo.ToString(), bimestres));


            var ocorrencias = await mediator.Send(new ObterOcorenciasPorTurmaEAlunoQuery(parametros.TurmaId, parametros.AlunoCodigo, periodoInicioFim.DataInicio, periodoInicioFim.DataFim));

            var relatorioDto = await mediator.Send(new ObterRelatorioAcompanhamentoAprendizagemQuery(turma, alunosEol, professores, acompanhmentosAlunos, frequenciaAlunos, ocorrencias, parametros, quantidadeAulasDadas, periodoInicioFim.Id));

            await mediator.Send(new GerarRelatorioHtmlCommand("RelatorioAcompanhamentoAprendizagem", relatorioDto, filtro.CodigoCorrelacao));
        }

        private async Task<PeriodoEscolarDto> ObterInicioFimPeriodo(long tipoCalendarioId, int[] bimestres, int semestre)
        {
            var periodosEscolares = await mediator.Send(new ObterPeriodosEscolaresPorTipoCalendarioQuery(tipoCalendarioId));
            int ano = periodosEscolares.FirstOrDefault().PeriodoInicio.Year;

            if (semestre == 1)
            {
                var periodoEscolar = periodosEscolares.FirstOrDefault(p => p.Bimestre == bimestres.Last());
                return new PeriodoEscolarDto()
                {
                    Id = periodoEscolar.Id,
                    DataInicio = new DateTime(ano, 1, 1),
                    DataFim = periodoEscolar.PeriodoFim
                };
            }
            else
            {
                var periodoEscolar = periodosEscolares.FirstOrDefault(p => p.Bimestre == bimestres.First());
                return new PeriodoEscolarDto()
                {
                    Id = periodoEscolar.Id,
                    DataInicio = periodoEscolar.PeriodoInicio,
                    DataFim = new DateTime(ano, 12, 31)
                };
            }
        }

        private static int[] ObterBimestresPorSemestre(int semestre)
        {
            if (semestre == 1)
                return new int[] { 1, 2 };
            else return new int[] { 3, 4 };
        }
    }
}
