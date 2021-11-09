using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class RelatorioAcompanhamentoFrequenciaUseCase : IRelatorioAcompanhamentoFrequenciaUseCase
    {
        public readonly IMediator mediator;
        private readonly IAlunoRepository alunoRepository;
        public RelatorioAcompanhamentoFrequenciaUseCase(IMediator mediator, IAlunoRepository alunoRepository)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this.alunoRepository = alunoRepository ?? throw new ArgumentNullException(nameof(alunoRepository));
        }
        public async Task Executar(FiltroRelatorioDto request)
        {
            request.RotaErro = RotasRabbitSGP.RotaRelatoriosComErroAcompanhamentoFrequencia;
            var codigoAlunosTodos = new List<string>();
            var filtroRelatorio = request.ObterObjetoFiltro<FiltroAcompanhamentoFrequenciaJustificativaDto>();
            var relatorio = new RelatorioFrequenciaIndividualDto();
            await MapearCabecalho(relatorio, filtroRelatorio);
            if (filtroRelatorio.AlunosCodigos.Contains("-99"))
            {
                var alunos = await mediator.Send(new ObterAlunosPorTurmaQuery() { TurmaCodigo = filtroRelatorio.TurmaCodigo });
                foreach (var item in alunos)
                {
                    codigoAlunosTodos.Add(item.CodigoAluno.ToString());
                }
            }
            else
                codigoAlunosTodos = filtroRelatorio.AlunosCodigos;

            var alunosSelecionados = await alunoRepository.ObterNomesAlunosPorCodigos(codigoAlunosTodos.ToArray());
            if (alunosSelecionados != null && alunosSelecionados.Any())
            {
                var dadosFrequencia = await mediator.Send(new ObterFrequenciaAlunoPorCodigoBimestreQuery(filtroRelatorio.Bimestre, codigoAlunosTodos.ToArray()));
                var dadosAusencia = await mediator.Send(new ObterAusenciaPorAlunoTurmaBimestreQuery(codigoAlunosTodos.ToArray(), filtroRelatorio.TurmaCodigo, filtroRelatorio.Bimestre));
                MapearAlunos(alunosSelecionados, relatorio, dadosFrequencia, dadosAusencia);
            }
            await mediator.Send(new GerarRelatorioHtmlParaPdfCommand("RelatorioFrequenciaIndividual", relatorio, request.CodigoCorrelacao));
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
                            NomeBimestre = item.NomeBimestre,
                        };

                        bimestre.DadosFrequencia = new RelatorioFrequenciaIndividualDadosFrequenciasDto
                        {
                            TotalAulasDadas = item.TotalAula,
                            TotalPresencas = item.TotalPresencas,
                            TotalRemoto = item.TotalRemotos,
                            TotalAusencias = item.TotalAusencias,
                            TotalCompensacoes = item.TotalCompensacoes,
                            PercentualFrequencia = item.PercentualFrequencia,
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
                                        MotivoAusencia = ausencia.MotivoAusencia,
                                    };

                                    bimestre.Justificativas.Add(justificativa);
                                }
                            }
                        }
                        aluno.Bimestres.Add(bimestre);
                    }
                }
            }
        }
        private async Task MapearCabecalho(RelatorioFrequenciaIndividualDto relatorio, FiltroAcompanhamentoFrequenciaJustificativaDto filtroRelatorio)
        {
            relatorio.RF = filtroRelatorio.UsuarioRF;
            relatorio.Usuario = filtroRelatorio.UsuarioNome;
            relatorio.DreNome = await ObterNomeDre(filtroRelatorio.DreCodigo);
            relatorio.UeNome = await ObterNomeUe(filtroRelatorio.UeCodigo);
            var turma = await mediator.Send(new ObterTurmaPorCodigoQuery(filtroRelatorio.TurmaCodigo));
            relatorio.ehInfantil = turma != null && turma.ModalidadeCodigo == Modalidade.Infantil;
        }
        private void MapearAlunos(IEnumerable<AlunoNomeDto> alunos, RelatorioFrequenciaIndividualDto relatorio, IEnumerable<FrequenciaAlunoConsolidadoDto> dadosFrequenciaDto, IEnumerable<AusenciaBimestreDto> ausenciaBimestreDto)
        {
            foreach (var aluno in alunos)
            {
                var relatorioFrequenciaIndividualAlunosDto = new RelatorioFrequenciaIndividualAlunosDto
                {
                    NomeAluno = aluno.Nome + $"({aluno.Codigo})",
                    CodigoAluno = aluno.Codigo
                };
                
                if (relatorio != null)
                {
                    MapearBimestre(dadosFrequenciaDto, ausenciaBimestreDto, relatorioFrequenciaIndividualAlunosDto);
                }
                relatorio.Alunos.Add(relatorioFrequenciaIndividualAlunosDto);
            }
        }
        private async Task<string> ObterNomeDre(string dreCodigo)
        {
            var dre = await mediator.Send(new ObterDrePorCodigoQuery(dreCodigo));
            return dre.Abreviacao;
        }

        private async Task<string> ObterNomeUe(string ueCodigo)
        {
            var ue = await mediator.Send(new ObterUePorCodigoQuery(ueCodigo));
            return ue.NomeRelatorio;
        }

    }
}
