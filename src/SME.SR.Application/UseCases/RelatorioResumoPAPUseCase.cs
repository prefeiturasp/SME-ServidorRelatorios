using MediatR;
using SME.SR.Infra;
using System;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioResumoPAPUseCase : IRelatorioResumoPAPUseCase
    {
        private readonly IMediator mediator;

        public RelatorioResumoPAPUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtros = request.ObterObjetoFiltro<FiltroRelatorioResumoPAPDto>();

            // Obter dados de dre e ue
            var dreUe = await ObterDadosDreUe(filtros);

            // Obter dados de aluno
            var alunos = await ObterDadosAlunos(filtros);

            // Obter seções
            var secoes = await mediator.Send(new ObterRelatorioRecuperacaoParalelaAlunoSecaoQuery()
            { TurmaCodigo = filtros.TurmaCodigo, AlunoCodigo = filtros.AlunoCodigo, Semestre = filtros.Semestre });

            var turma = await mediator.Send(new ObterTurmaQuery(filtros.TurmaCodigo));
            if (turma == null)
                throw new NegocioException($"Não foi possível obter os dados da turma {filtros.TurmaCodigo}");

            var relatorioRecuperacaoParalelaDto = new RelatorioRecuperacaoParalelaDto(dreUe.DreNome, dreUe.UeNome)
            {
                Semestre = filtros.Semestre,
                UsuarioNome = filtros.UsuarioNome,
                AnoLetivo = secoes.FirstOrDefault().AnoLetivo,
                UsuarioRF = filtros.UsuarioRf
            };

            //Obter Dados da turma Regular
            var turmaRegularDaTurmaRec = await mediator.Send(new ObterAlunosTurmasRegularesPorTurmaRecuperacaoCodigoQuery(long.Parse(filtros.TurmaCodigo)));
            if (turmaRegularDaTurmaRec == null || !turmaRegularDaTurmaRec.Any())
                throw new NegocioException($"Não foi possível obter as turmas regulares da turma {filtros.TurmaCodigo}.");


            // Prencher Alunos
            PreencherAlunos(alunos, filtros, secoes, relatorioRecuperacaoParalelaDto, turmaRegularDaTurmaRec, turma);

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioRecuperacaoParalela", relatorioRecuperacaoParalelaDto, request.CodigoCorrelacao));
        }

        private async Task<DreUe> ObterDadosDreUe(FiltroRelatorioRecuperacaoParalelaDto filtros)
        {
            var dreUe = await mediator.Send(new ObterDreUePorTurmaQuery() { CodigoTurma = filtros.TurmaCodigo });

            if (dreUe == null)
                throw new NegocioException($"Não foi possível localizar dados do Dre e Ue para a turma {filtros.TurmaCodigo}");
            return dreUe;
        }
    }
}
