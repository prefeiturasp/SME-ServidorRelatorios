using MediatR;
using SME.SR.Application.Queries;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioSondagemComponentesPorTurmaUseCase : IRelatorioSondagemComponentesPorTurmaUseCase
    {
        private readonly IMediator mediator;

        public RelatorioSondagemComponentesPorTurmaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<string> Executar(FiltroRelatorioSincronoDto request)
        {
            var filtros = request.ObterObjetoFiltro<RelatorioSondagemComponentesPorTurmaFiltroDto>();

            var dataPeriodoFim = await ObterDataPeriodoFim(filtros.AnoLetivo, filtros.Semestre, filtros.Bimestre);
            var alunosDaTurma = await ObterAlunos((int)filtros.TurmaCodigo, dataPeriodoFim);

            if (alunosDaTurma?.Any() != true)
                throw new NegocioException("Não foi possível localizar os alunos da turma.");

            var relatorio = await ObterDadosRelatorio(filtros, alunosDaTurma);

            if (relatorio == null)
                throw new NegocioException("Não foi possível localizar dados com os filtros informados.");

            if (filtros.AnoLetivo >= 2022)
            {
                return await ObtenhaRelatorioPaginado(relatorio);
            }

            return await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioSondagemComponentesPorTurma", relatorio, Guid.NewGuid(), envioPorRabbit: false));
        }

        private async Task<IEnumerable<Aluno>> ObterAlunos(int codigoTurma, PeriodoCompletoSondagemDto periodo)
        {
            const int DEFICIENTE = 1;
            var alunos = await mediator.Send(new ObterAlunosMatriculasPorTurmasQuery(new int[] { codigoTurma }));

            alunos = alunos.Where(a => ((a.Ativo && a.DataMatricula.Date < (periodo?.PeriodoFim.Date ?? DateTime.Now.Date)) ||
                                            (!a.Ativo && a.DataMatricula.Date < (periodo?.PeriodoFim.Date ?? DateTime.Now.Date) && a.DataSituacao.Date >= (periodo?.PeriodoInicio.Date ?? DateTime.Now.Date))) &&
                                            (SituacaoMatriculaAluno)a.CodigoSituacaoMatricula != SituacaoMatriculaAluno.VinculoIndevido);
            
            return alunos.Select(aluno => new Aluno()
            {
                CodigoAluno = aluno.CodigoAluno,
                CodigoSituacaoMatricula = (SituacaoMatriculaAluno)aluno.CodigoSituacaoMatricula,
                CodigoTurma = aluno.CodigoTurma,
                DataMatricula = aluno.DataMatricula,
                DataNascimento = aluno.DataNascimento,
                DataSituacao = aluno.DataSituacao,
                NomeAluno = aluno.NomeAluno,
                NomeSocialAluno = aluno.NomeSocialAluno,
                NumeroAlunoChamada = aluno.NumeroAlunoChamada,
                PossuiDeficiencia = aluno.PossuiDeficiencia == DEFICIENTE,
                SituacaoMatricula = aluno.SituacaoMatricula,
                
            });
        }

        private async Task<string> ObtenhaRelatorioPaginado(RelatorioSondagemComponentesPorTurmaRelatorioDto dtoSondagem)
        {
            var preparo = new PreparadorDeRelatorioPaginadoSondagemPorTurmaMatematica(dtoSondagem);
            var dto = preparo.ObtenhaRelatorioPaginadoDto();

            return await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioPaginado/Index", dto, Guid.NewGuid(), envioPorRabbit: false));
        }

        private async Task<PeriodoCompletoSondagemDto> ObterDataPeriodoFim(int anoLetivo, int semestre, int bimestre)
        {
            if (anoLetivo < 2022)
                return await mediator.Send(new ObterDataPeriodoFimSondagemPorSemestreAnoLetivoQuery(semestre, anoLetivo));
            else
                return await mediator.Send(new ObterDataPeriodoFimSondagemPorBimestreAnoLetivoQuery((semestre > 0 ? semestre : bimestre), anoLetivo));
        }

        private async Task<RelatorioSondagemComponentesPorTurmaRelatorioDto> ObterDadosRelatorio(RelatorioSondagemComponentesPorTurmaFiltroDto filtros, IEnumerable<Aluno> alunos)
        {
            return await mediator.Send(
               new ObterRelatorioSondagemComponentesPorTurmaQuery()
               {
                    AnoLetivo = filtros.AnoLetivo,
                    ComponenteCurricular = filtros.ComponenteCurricularId,
                    DreCodigo = filtros.DreCodigo,
                    Proficiencia = filtros.ProficienciaId,
                    Semestre = filtros.Semestre,
                    Bimestre = filtros.Bimestre,
                    TurmaCodigo = filtros.TurmaCodigo,
                    UeCodigo = filtros.UeCodigo,
                    UsuarioRF = filtros.UsuarioRF,
                    alunos = alunos,
                    Ano = filtros.Ano
               });
        }
    }
}
