using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioRecuperacaoParalelaUseCase : IRelatorioRecuperacaoParalelaUseCase
    {
        private readonly IMediator mediator;

        public RelatorioRecuperacaoParalelaUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            var filtros = request.ObterObjetoFiltro<FiltroRelatorioRecuperacaoParalelaDto>();

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

        private static void PreencherAlunos(List<Aluno> alunos, FiltroRelatorioRecuperacaoParalelaDto filtros, IEnumerable<RelatorioRecuperacaoParalelaRetornoQueryDto> secoes,
            RelatorioRecuperacaoParalelaDto relatorioRecuperacaoParalelaDto, IEnumerable<AlunoTurmaRegularRetornoDto> turmaRegularAlunos, Turma turma)
        {
            var alunosCodigosComSecao = secoes.Select(a => int.Parse(a.AlunoCodigo)).Distinct();
            
            var alunosDto = new List<RelatorioRecuperacaoParalelaAlunoDto>();
            foreach (var item in alunos.Where(a => alunosCodigosComSecao.Contains(a.CodigoAluno)))
            {

                var turmaRegular = turmaRegularAlunos.FirstOrDefault(a => a.AlunoCodigo == item.CodigoAluno)?.TurmaNome;

                var nomeAlunoComNumeroChamada = $"{item.NomeAluno} Nº {item.NumeroAlunoChamada}";

                var relatorioRecuperacaoParalelaAlunoDto = new RelatorioRecuperacaoParalelaAlunoDto(
                        nomeAlunoComNumeroChamada, turma.Nome, item.DataNascimento.ToString("dd/MM/yyyy"),
                        item.CodigoAluno.ToString(), turmaRegular.ToString(),  item.SituacaoMatricula) ;

                // Secoes
                AtribuirSecoes(secoes.Where(a => a.TurmaCodigo == filtros.TurmaCodigo && a.AlunoCodigo == item.CodigoAluno.ToString()), relatorioRecuperacaoParalelaAlunoDto);
                alunosDto.Add(relatorioRecuperacaoParalelaAlunoDto);
            }

            relatorioRecuperacaoParalelaDto.Alunos = alunosDto.OrderBy(x => x.AlunoNome).ToList();
        }

        private static void AtribuirSecoes(IEnumerable<RelatorioRecuperacaoParalelaRetornoQueryDto> secoes,
            RelatorioRecuperacaoParalelaAlunoDto relatorioRecuperacaoParalelaAlunoDto)
        {
            var secoesDto = new List<RelatorioRecuperacaoParalelaAlunoSecaoDto>();
            foreach (var secao in secoes)
            {
                secoesDto.Add(new RelatorioRecuperacaoParalelaAlunoSecaoDto(secao.SecaoNome, secao.SecaoValor));
            }

            relatorioRecuperacaoParalelaAlunoDto.Secoes = secoesDto;
        }

        private async Task<List<Aluno>> ObterDadosAlunos(FiltroRelatorioRecuperacaoParalelaDto filtros)
        {
            var alunos = new List<Aluno>();
            if (!string.IsNullOrEmpty(filtros.AlunoCodigo))
            {
                var aluno = await mediator.Send(new ObterDadosAlunoQuery()
                {
                    CodigoAluno = filtros.AlunoCodigo,
                    CodigoTurma = filtros.TurmaCodigo
                });

                if (aluno == null)
                    throw new NegocioException($"Não foi possível localizar dados do aluno {filtros.AlunoCodigo}");
                aluno.NomeAluno = aluno.ObterNomeFinal();

                alunos.Add(aluno);
            }
            else
            {
                alunos = (await mediator.Send(new ObterAlunosPorTurmaQuery()
                {
                    TurmaCodigo = filtros.TurmaCodigo
                })).ToList();
            }

            return alunos;
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