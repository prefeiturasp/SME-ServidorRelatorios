using MediatR;
using Newtonsoft.Json;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SME.SR.Infra.Enumeradores;

namespace SME.SR.Application
{
    public class RelatorioConselhoClasseAtaFinalUseCase : IRelatorioConselhoClasseAtaFinalUseCase
    {
        private readonly IMediator mediator;

        public RelatorioConselhoClasseAtaFinalUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            try
            {
                var parametros = request.ObterObjetoFiltro<FiltroConselhoClasseAtaFinalDto>();

                var turma = await ObterTurma(parametros.TurmaCodigo);
                var tipoCalendarioId = await ObterIdTipoCalendario(turma.ModalidadeTipoCalendario, turma.AnoLetivo, turma.Semestre);
                var periodosEscolares = await ObterPeriodosEscolares(tipoCalendarioId);

                var cabecalho = await ObterCabecalho(parametros.TurmaCodigo);
                var alunos = await ObterAlunos(parametros.TurmaCodigo);
                var componentesCurriculares = await ObterComponentesCurriculares(parametros.TurmaCodigo);
                var notasFinais = await ObterNotasFinaisPorTurma(parametros.TurmaCodigo);
                var frequenciaAlunos = await ObterFrequenciaComponente(parametros.TurmaCodigo, tipoCalendarioId);

                var pareceresConclusivos = await ObterPareceresConclusivos(parametros.TurmaCodigo);

                var dadosRelatorio = await MontarEstruturaRelatorio(cabecalho, alunos, componentesCurriculares, notasFinais, frequenciaAlunos, pareceresConclusivos, periodosEscolares);
                var jsonRelatorio = JsonConvert.SerializeObject(dadosRelatorio);

                await mediator.Send(new GerarRelatorioAssincronoCommand("/sgp/RelatorioConselhoAta/ConselhoAta", jsonRelatorio, FormatoEnum.Pdf, request.CodigoCorrelacao));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async Task<ConselhoClasseAtaFinalDto> MontarEstruturaRelatorio(ConselhoClasseAtaFinalCabecalhoDto cabecalho, IEnumerable<AlunoSituacaoAtaFinalDto> alunos, IEnumerable<ComponenteCurricularPorTurma> componentesCurriculares, IEnumerable<NotaConceitoBimestreComponente> notasFinais, IEnumerable<FrequenciaAluno> frequenciaAlunos, IEnumerable<ConselhoClasseParecerConclusivo> pareceresConclusivos, IEnumerable<PeriodoEscolar> periodosEscolares)
        {
            var relatorio = new ConselhoClasseAtaFinalDto();

            relatorio.Cabecalho = cabecalho;
            var gruposMatrizes = componentesCurriculares.GroupBy(c => c.GrupoMatriz);

            MontarEstruturaGruposMatriz(ref relatorio, gruposMatrizes, periodosEscolares);
            MontarEstruturaLinhas(ref relatorio, alunos, gruposMatrizes, periodosEscolares, notasFinais, frequenciaAlunos);
            return relatorio;
        }

        private void MontarEstruturaLinhas(ref ConselhoClasseAtaFinalDto relatorio, IEnumerable<AlunoSituacaoAtaFinalDto> alunos, IEnumerable<IGrouping<ComponenteCurricularGrupoMatriz, ComponenteCurricularPorTurma>> gruposMatrizes, IEnumerable<PeriodoEscolar> periodosEscolares, IEnumerable<NotaConceitoBimestreComponente> notasFinais, IEnumerable<FrequenciaAluno> frequenciaAlunos)
        {
            foreach(var aluno in alunos)
            {
                var linhaDto = new ConselhoClasseAtaFinalLinhaDto()
                {
                    Id = aluno.CodigoAluno,
                    Nome = aluno.NomeAluno
                };

                foreach (var grupoMatriz in gruposMatrizes)
                {
                    foreach(var componente in grupoMatriz)
                    {
                        var coluna = 0;
                        // Monta Colunas notas dos bimestres
                        foreach(var periodoEscolar in periodosEscolares)
                        {
                            var notaConceito = notasFinais.FirstOrDefault(c => c.AlunoCodigo == aluno.CodigoAluno.ToString()
                                                    && c.ComponenteCurricularCodigo == componente.CodDisciplina
                                                    && c.Bimestre == periodoEscolar.Bimestre);

                            linhaDto.AdicionaCelula(grupoMatriz.Key.Id,
                                                    componente.CodDisciplina,
                                                    notaConceito?.NotaConceito ?? "",
                                                    ++coluna);
                        }

                        // Monta colunas frequencia SF - F - CA - %
                        var frequenciaAluno = frequenciaAlunos.FirstOrDefault(c => c.CodigoAluno == aluno.CodigoAluno.ToString()
                                                                        && c.DisciplinaId == componente.CodDisciplina.ToString());
                        linhaDto.AdicionaCelula(grupoMatriz.Key.Id,
                                                componente.CodDisciplina,
                                                frequenciaAluno?.TotalAulas.ToString() ?? "0",
                                                ++coluna);
                        linhaDto.AdicionaCelula(grupoMatriz.Key.Id,
                                                componente.CodDisciplina,
                                                frequenciaAluno?.TotalAusencias.ToString() ?? "0",
                                                ++coluna);
                        linhaDto.AdicionaCelula(grupoMatriz.Key.Id,
                                                componente.CodDisciplina,
                                                frequenciaAluno?.TotalCompensacoes.ToString() ?? "0",
                                                ++coluna);
                        linhaDto.AdicionaCelula(grupoMatriz.Key.Id,
                                                componente.CodDisciplina,
                                                $"{frequenciaAluno?.PercentualFrequencia ?? 100} %",
                                                ++coluna);
                    }
                }

                relatorio.Linhas.Add(linhaDto);
            }
        }

        private void MontarEstruturaGruposMatriz(ref ConselhoClasseAtaFinalDto relatorio, IEnumerable<IGrouping<ComponenteCurricularGrupoMatriz, ComponenteCurricularPorTurma>> gruposMatrizes, IEnumerable<PeriodoEscolar> periodosEscolares)
        {
            foreach(var grupoMatriz in gruposMatrizes)
            {
                var grupoMatrizDto = new ConselhoClasseAtaFinalGrupoDto()
                {
                    Id = grupoMatriz.Key.Id,
                    Nome = grupoMatriz.Key.Nome
                };

                foreach (var componenteCurricular in grupoMatriz)
                {
                    grupoMatrizDto.AdicionarComponente(componenteCurricular.CodDisciplina, componenteCurricular.Disciplina, periodosEscolares.Select(s => s.Bimestre));
                }

                relatorio.GruposMatriz.Add(grupoMatrizDto);
            }
        }

        private async Task<IEnumerable<PeriodoEscolar>> ObterPeriodosEscolares(long tipoCalendarioId)
            => await mediator.Send(new ObterPeriodosEscolaresPorTipoCalendarioQuery(tipoCalendarioId));

        private async Task<long> ObterIdTipoCalendario(ModalidadeTipoCalendario modalidade, int anoLetivo, int semestre)
            => await mediator.Send(new ObterIdTipoCalendarioPorAnoLetivoEModalidadeQuery(anoLetivo, modalidade, semestre));

        private async Task<IEnumerable<ConselhoClasseParecerConclusivo>> ObterPareceresConclusivos(string turmaCodigo)
            => await mediator.Send(new ObterParecerConclusivoPorTurmaQuery(turmaCodigo));

        private async Task<ConselhoClasseAtaFinalCabecalhoDto> ObterCabecalho(string turmaCodigo)
            => await mediator.Send(new ObterAtaFinalCabecalhoQuery(turmaCodigo));

        private async Task<Turma> ObterTurma(string turmaCodigo)
            => await mediator.Send(new ObterTurmaQuery(turmaCodigo));

        private async Task<IEnumerable<FrequenciaAluno>> ObterFrequenciaComponente(string turmaCodigo, long tipoCalendarioId)
            => await mediator.Send(new ObterFrequenciaComponenteGlobalPorTurmaQuery(turmaCodigo, tipoCalendarioId));

        private async Task<IEnumerable<NotaConceitoBimestreComponente>> ObterNotasFinaisPorTurma(string turmaCodigo)
        {
            return await mediator.Send(new ObterNotasFinaisPorTurmaQuery(turmaCodigo));
        }

        private async Task<IEnumerable<AlunoSituacaoAtaFinalDto>> ObterAlunos(string turmaCodigo)
        {
            var alunos = await mediator.Send(new ObterAlunosSituacaoPorTurmaQuery(turmaCodigo)); 
            return alunos.Select(a => new AlunoSituacaoAtaFinalDto(a));
        }

        private async Task<IEnumerable<ComponenteCurricularPorTurma>> ObterComponentesCurriculares(string turmaCodigo)
            => await mediator.Send(new ObterComponentesCurricularesPorTurmaQuery(turmaCodigo));
    }
}
