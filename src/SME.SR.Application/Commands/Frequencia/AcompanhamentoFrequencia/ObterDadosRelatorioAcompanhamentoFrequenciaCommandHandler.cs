using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Dtos;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace SME.SR.Application
{
    public class ObterDadosRelatorioAcompanhamentoFrequenciaCommandHandler : IRequestHandler<ObterDadosRelatorioAcompanhamentoFrequenciaCommand, RelatorioFrequenciaIndividualDto>
    {
        private readonly IMediator mediator;
        private readonly IAlunoRepository alunoRepository;
        public ObterDadosRelatorioAcompanhamentoFrequenciaCommandHandler(IMediator mediator, IAlunoRepository alunoRepository)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
        }
        public async Task<RelatorioFrequenciaIndividualDto> Handle(ObterDadosRelatorioAcompanhamentoFrequenciaCommand request, CancellationToken cancellationToken)
        {
            var relatorio = new RelatorioFrequenciaIndividualDto();
            var alunosSelecionados = new List<AlunoNomeDto>();
            var turma = await ObterDadosDaTurma(request.FiltroRelatorio.TurmaCodigo);
            await MapearCabecalho(relatorio, request.FiltroRelatorio, turma);
            relatorio.ehTodosBimestre = request.FiltroRelatorio.Bimestre.Equals("-99");
            if (request.FiltroRelatorio.AlunosCodigos.Contains("-99"))
            {
                var alunos = await mediator.Send(new ObterAlunosPorTurmaQuery() { TurmaCodigo = request.FiltroRelatorio.TurmaCodigo });
                foreach (var aluno in alunos.OrderBy(x => x.NomeAluno))
                {
                    var dto = new AlunoNomeDto
                    {
                        Nome = aluno.NomeAluno,
                        Codigo = aluno.CodigoAluno.ToString()
                    };
                    alunosSelecionados.Add(dto);
                }
            }
            else
                alunosSelecionados = (await alunoRepository.ObterNomesAlunosPorCodigos(request.FiltroRelatorio.AlunosCodigos.ToArray())).ToList();

            if (alunosSelecionados != null && alunosSelecionados.Any())
            {
                var dadosFrequencia = await mediator.Send(new ObterFrequenciaAlunoPorCodigoBimestreQuery(request.FiltroRelatorio.Bimestre, alunosSelecionados.Select(s => s.Codigo).ToArray(), request.FiltroRelatorio.TurmaCodigo, TipoFrequenciaAluno.Geral));
                if (dadosFrequencia == null || !dadosFrequencia.Any())
                    throw new NegocioException("Nenhuma informação para os filtros informados.");

                var dadosAusencia = await mediator.Send(new ObterAusenciaPorAlunoTurmaBimestreQuery(alunosSelecionados.Select(s => s.Codigo).ToArray(), request.FiltroRelatorio.TurmaCodigo, request.FiltroRelatorio.Bimestre));
                await MapearAlunos(alunosSelecionados, relatorio, dadosFrequencia, dadosAusencia, turma);
            }
            return relatorio;
        }

        private void MapearBimestre(IEnumerable<FrequenciaAlunoConsolidadoDto> dadosFrequenciaDto, IEnumerable<AusenciaBimestreDto> ausenciaBimestreDto, RelatorioFrequenciaIndividualAlunosDto aluno)
        {
            if (dadosFrequenciaDto != null && dadosFrequenciaDto.Any())
            {

                foreach (var item in dadosFrequenciaDto)
                {
                    if (aluno.CodigoAluno == item.CodigoAluno)
                    {
                        var bimestre = new RelatorioFrequenciaIndividualBimestresDto
                        {
                            NomeBimestre = item.BimestreFormatado,
                        };

                        bimestre.DadosFrequencia = new RelatorioFrequenciaIndividualDadosFrequenciasDto
                        {
                            TotalAulasDadas = item.TotalAula,
                            TotalPresencas = item.TotalPresencas,
                            TotalRemoto = item.TotalRemotos,
                            TotalAusencias = item.TotalAusencias,
                            TotalCompensacoes = item.TotalCompensacoes,
                            TotalPercentualFrequencia = Math.Round(item.TotalPercentualFrequencia, 0),
                            TotalPercentualFrequenciaFormatado = item.TotalPercentualFrequenciaFormatado
                        };

                        if (ausenciaBimestreDto != null && ausenciaBimestreDto.Any())
                        {
                            foreach (var ausencia in ausenciaBimestreDto)
                            {
                                if (item.CodigoAluno == ausencia.CodigoAluno && item.Bimestre == ausencia.Bimestre)
                                {
                                    var justificativa = new RelatorioFrequenciaIndividualJustificativasDto
                                    {
                                        DataAusencia = ausencia.DataAusencia.ToString("dd/MM/yyyy"),
                                        MotivoAusencia = UtilHtml.FormatarHtmlParaTexto(ausencia.MotivoAusencia),
                                    };

                                    bimestre.Justificativas.Add(justificativa);
                                }
                            }
                        }
                        aluno.Bimestres.Add(bimestre);
                    }

                }
                aluno.TotalAulasDadasFinal = aluno.Bimestres.Sum(x => x.DadosFrequencia.TotalAulasDadas);
                aluno.TotalAusenciasFinal = aluno.Bimestres.Sum(x => x.DadosFrequencia.TotalAusencias);
                aluno.TotalCompensacoesFinal = aluno.Bimestres.Sum(x => x.DadosFrequencia.TotalCompensacoes);
                aluno.TotalPresencasFinal = aluno.Bimestres.Sum(x => x.DadosFrequencia.TotalPresencas);
                aluno.TotalRemotoFinal = aluno.Bimestres.Sum(x => x.DadosFrequencia.TotalRemoto);
                aluno.TituloFinal = $"FINAL - {dadosFrequenciaDto.Where(x => x.CodigoAluno == aluno.CodigoAluno).FirstOrDefault().AnoBimestre}";
                aluno.PercentualFrequenciaFinal = aluno.Bimestres.Average(x => x.DadosFrequencia.TotalPercentualFrequencia);
            }
        }

        private async Task MapearCabecalho(RelatorioFrequenciaIndividualDto relatorio, FiltroAcompanhamentoFrequenciaJustificativaDto filtroRelatorio, Turma turma)
        {
            var dadosDreUe = await ObterNomeDreUe(filtroRelatorio.TurmaCodigo);

            relatorio.RF = filtroRelatorio.UsuarioRF;
            relatorio.Usuario = filtroRelatorio.UsuarioNome;
            relatorio.DreNome = dadosDreUe.DreNome;
            relatorio.UeNome = dadosDreUe.UeNome;
            relatorio.ComponenteNome = await ObterNomeComponente(filtroRelatorio.ComponenteCurricularId);
            relatorio.TurmaNome = turma.NomePorFiltroModalidade(null);
            relatorio.ehInfantil = turma != null && turma.ModalidadeCodigo == Modalidade.Infantil;
        }
        private async Task<Turma> ObterDadosDaTurma(string turmaCodigo)
        {
            return await mediator.Send(new ObterTurmaPorCodigoQuery(turmaCodigo));
        }
        private async Task<string> ObterNomeComponente(string componenteCodigo)
        {
            var id = Convert.ToInt64(componenteCodigo);
            return await mediator.Send(new ObterNomeComponenteCurricularPorIdQuery(id));
        }
        private async Task MapearAlunos(IEnumerable<AlunoNomeDto> alunos, RelatorioFrequenciaIndividualDto relatorio, IEnumerable<FrequenciaAlunoConsolidadoDto> dadosFrequenciaDto, IEnumerable<AusenciaBimestreDto> ausenciaBimestreDto, Turma turma)
        {

            foreach (var aluno in alunos.OrderBy(x => x.Nome))
            {
                if (dadosFrequenciaDto.Where(x => x.CodigoAluno == aluno.Codigo).Any())
                {
                    var alunoAtivo = (await alunoRepository.ObterAlunosPorTurmaCodigoParaRelatorioAcompanhamentoAprendizagem(long.Parse(turma.Codigo), long.Parse(aluno.Codigo), turma.AnoLetivo)).FirstOrDefault().Ativo;
                    var situacaoAluno = alunoAtivo ? string.Empty : " - Inativo";
                    var relatorioFrequenciaIndividualAlunosDto = new RelatorioFrequenciaIndividualAlunosDto
                    {
                        NomeAluno =  $"{aluno.Nome} ({aluno.Codigo}) {situacaoAluno}",
                        CodigoAluno = aluno.Codigo
                    };

                    if (relatorio != null)
                    {
                        MapearBimestre(dadosFrequenciaDto, ausenciaBimestreDto, relatorioFrequenciaIndividualAlunosDto);
                    }
                    relatorio.Alunos.Add(relatorioFrequenciaIndividualAlunosDto);
                }
            }
        }
        private async Task<DreUe> ObterNomeDreUe(string turmaCodigo)
        {
            return await mediator.Send(new ObterDreUePorTurmaQuery(turmaCodigo));
        }
    }
}
