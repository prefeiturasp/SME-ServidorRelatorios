using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using SME.SR.Data;

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

            var relatorioRecuperacaoParalelaDto = new RelatorioRecuperacaoParalelaDto(dreUe.DreNome, dreUe.UeNome)
            {
                Semestre = filtros.Semestre,
                Data = new DateTime().ToString(CultureInfo.CurrentCulture),
                UsuarioNome = filtros.UsuarioNome,
                AnoLetivo = secoes.FirstOrDefault().AnoLetivo,
                UsuarioRF = filtros.UsuarioRf
            };

            var alunosDto = new List<RelatorioRecuperacaoParalelaAlunoDto>();
            foreach (var item in alunos)
            {
                var relatorioRecuperacaoParalelaAlunoDto = new RelatorioRecuperacaoParalelaAlunoDto(
                    item.NomeAluno, filtros.TurmaCodigo, item.DataNascimento.ToString(),
                    item.CodigoAluno.ToString(), item.CodigoTurma.ToString(), item.SituacaoMatricula);
                

                var secoesDto = new List<RelatorioRecuperacaoParalelaAlunoSecaoDto>();
                foreach (var secao in secoes)
                {
                    secoesDto.Add(new RelatorioRecuperacaoParalelaAlunoSecaoDto(secao.SecaoNome, secao.SecaoValor));
                }

                relatorioRecuperacaoParalelaAlunoDto.Secoes = secoesDto;
                alunosDto.Add(relatorioRecuperacaoParalelaAlunoDto);
            }

            relatorioRecuperacaoParalelaDto.Alunos = alunosDto;

            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioRecuperacaoParalela", relatorioRecuperacaoParalelaDto, request.CodigoCorrelacao));

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
            var dreUe = await mediator.Send(new ObterDreUePorTurmaQuery() {CodigoTurma = filtros.TurmaCodigo});

            if (dreUe == null)
                throw new NegocioException($"Não foi possível localizar dados do Dre e Ue para a turma {filtros.TurmaCodigo}");
            return dreUe;
        }
    }
}