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
            
            var turma = await mediator.Send(new ObterTurmaPorIdQuery(parametros.TurmaId));

            if (turma == null)
                throw new NegocioException("Turma não encontrada");

            var turmaCodigo = new string[1] { turma.Codigo };
            var professores = await mediator.Send(new ObterProfessorTitularComponenteCurricularPorTurmaQuery(turmaCodigo));

            var alunosEol = await mediator.Send(new ObterAlunosPorTurmaAcompanhamentoApredizagemQuery(turma.Codigo, parametros.AlunoCodigo));
            if (alunosEol == null || !alunosEol.Any())
                throw new NegocioException("Alunos não encontrados");

            
            var acompanhmentosAlunos = await mediator.Send(new ObterAcompanhamentoAprendizagemPorTurmaESemestreQuery(parametros.TurmaId, parametros.AlunoCodigo.ToString(), parametros.Semestre));
            if (acompanhmentosAlunos == null)
                throw new NegocioException("Acompanhamentos não encontrados");
            
            var bimestres = ObterBimestresPorSemestre(parametros.Semestre);
            
            var frequenciaAlunos = await mediator.Send(new ObterFrequenciaGeralAlunosPorTurmaEBimestreQuery(parametros.TurmaId, parametros.AlunoCodigo.ToString(), bimestres));
            if (frequenciaAlunos == null || !frequenciaAlunos.Any())
                throw new NegocioException("Frequências não encontradas");
           
            var registrosIndividuais = await mediator.Send(new ObterRegistroIndividualPorTurmaEAlunoQuery(parametros.TurmaId, parametros.AlunoCodigo));         
                       
            var Ocorrencias = await mediator.Send(new ObterOcorenciasPorTurmaEAlunoQuery(parametros.TurmaId, parametros.AlunoCodigo));            

            try
            {
                var relatorioDto = new RelatorioAcompanhamentoAprendizagemDto();

                relatorioDto = await mediator.Send(new ObterRelatorioAcompanhamentoAprendizagemQuery(alunosEol, professores, acompanhmentosAlunos, frequenciaAlunos, registrosIndividuais, Ocorrencias));

                await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioAcompanhamentoAprendizagem", relatorioDto, filtro.CodigoCorrelacao));

            }
            catch (Exception ex)
            {

                throw ex;
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
