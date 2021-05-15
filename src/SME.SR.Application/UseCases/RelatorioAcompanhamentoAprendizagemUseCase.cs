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

            var alunosEol = await mediator.Send(new ObterAlunosPorTurmaAcompanhamentoApredizagemQuery(turma.Codigo, parametros.AlunoCodigo));
            if (alunosEol == null || !alunosEol.Any())
                throw new NegocioException("Nenhuma informação para os filtros informados.");

            var acompanhmentosAlunos = await mediator.Send(new ObterAcompanhamentoAprendizagemPorTurmaESemestreQuery(parametros.TurmaId, parametros.AlunoCodigo.ToString(), parametros.Semestre));

            var bimestres = ObterBimestresPorSemestre(parametros.Semestre);

            var tipoCalendarioId = await mediator.Send(new ObterIdTipoCalendarioPorAnoLetivoEModalidadeQuery(turma.AnoLetivo, turma.ModalidadeTipoCalendario, turma.Semestre));

            var quantidadeAulasDadas = await mediator.Send(new ObterQuantiaddeAulasDadasPorTurmaEBimestreQuery(turma.Codigo, tipoCalendarioId, bimestres));
            
            var frequenciaAlunos = await mediator.Send(new ObterFrequenciaGeralAlunosPorTurmaEBimestreQuery(parametros.TurmaId, parametros.AlunoCodigo.ToString(), bimestres));
            
            var registrosIndividuais = await mediator.Send(new ObterRegistroIndividualPorTurmaEAlunoQuery(parametros.TurmaId, parametros.AlunoCodigo));

            var Ocorrencias = await mediator.Send(new ObterOcorenciasPorTurmaEAlunoQuery(parametros.TurmaId, parametros.AlunoCodigo));            

            var relatorioDto = await mediator.Send(new ObterRelatorioAcompanhamentoAprendizagemQuery(turma, alunosEol, professores, acompanhmentosAlunos, frequenciaAlunos, registrosIndividuais, Ocorrencias, parametros, quantidadeAulasDadas));

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioAcompanhamentoAprendizagem", relatorioDto, filtro.CodigoCorrelacao, gerarPaginacao: false));
        }

        private static int[] ObterBimestresPorSemestre(int semestre)
        {
            if (semestre == 1)
                return new int[] { 1, 2 };
            else return new int[] { 3, 4 };
        }
    }
}
