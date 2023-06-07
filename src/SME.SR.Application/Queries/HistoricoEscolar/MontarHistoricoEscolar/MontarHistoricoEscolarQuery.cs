using MediatR;
using SME.SR.Data;
using SME.SR.Data.Models;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Application
{
    public class MontarHistoricoEscolarQuery : IRequest<IEnumerable<HistoricoEscolarFundamentalDto>>
    {
        public MontarHistoricoEscolarQuery(Dre dre, Ue ue, IEnumerable<AreaDoConhecimento> areasConhecimento,
                                            IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>> componentesCurricularesTurmas,
                                            IEnumerable<ComponenteCurricularGrupoAreaOrdenacaoDto> grupoAreaOrdenacao,
                                            IEnumerable<AlunoTurmasHistoricoEscolarDto> alunosTurmas,
                                            IEnumerable<MediaFrequencia> mediasFrequencia,
                                            IEnumerable<IGrouping<string, NotasAlunoBimestre>> notas,
                                            IEnumerable<IGrouping<string, FrequenciaAluno>> frequencias,
                                            IEnumerable<TipoNotaCicloAno> tiposNota,
                                            IEnumerable<TransferenciaDto> transferencias,
                                            string[] turmasCodigo, CabecalhoDto cabecalho, LegendaDto legenda,
                                            DadosDataDto dadosData, FuncionarioDto dadosDiretor, FuncionarioDto dadosSecretario,
                                            IEnumerable<IGrouping<(long, Modalidade), UeConclusaoPorAlunoAno>> historicoUes,
                                            bool preencherDataImpressao, bool imprimirDadosResponsaveis,
                                            IEnumerable<FiltroHistoricoEscolarAlunosDto> filtroHistoricoAlunos)
        {
            Dre = dre;
            Ue = ue;
            AreasConhecimento = areasConhecimento;
            ComponentesCurricularesTurmas = componentesCurricularesTurmas;
            GrupoAreaOrdenacao = grupoAreaOrdenacao;
            AlunosTurmas = alunosTurmas;
            TurmasCodigo = turmasCodigo;
            Cabecalho = cabecalho;
            HistoricoUes = historicoUes;
            Notas = notas;
            Frequencias = frequencias;
            MediasFrequencia = mediasFrequencia;
            TiposNota = tiposNota;
            Transferencias = transferencias;
            Legenda = legenda;
            DadosData = dadosData;
            DadosDiretor = dadosDiretor;
            DadosSecretario = dadosSecretario;
            PreencherDataImpressao = preencherDataImpressao;
            ImprimirDadosResponsaveis = imprimirDadosResponsaveis;
            FiltroHistoricoAlunos = filtroHistoricoAlunos;
        }

        public Dre Dre { get; set; }
        public Ue Ue { get; set; }
        public IEnumerable<IGrouping<string, NotasAlunoBimestre>> Notas { get; set; }
        public IEnumerable<IGrouping<string, FrequenciaAluno>> Frequencias { get; set; }
        public IEnumerable<IGrouping<(long, Modalidade), UeConclusaoPorAlunoAno>> HistoricoUes { get; set; }
        public IEnumerable<MediaFrequencia> MediasFrequencia { get; set; }
        public IEnumerable<AreaDoConhecimento> AreasConhecimento { get; set; }
        public IEnumerable<ComponenteCurricularGrupoAreaOrdenacaoDto> GrupoAreaOrdenacao { get; set; }
        public IEnumerable<IGrouping<string, ComponenteCurricularPorTurma>> ComponentesCurricularesTurmas { get; set; }
        public IEnumerable<AlunoTurmasHistoricoEscolarDto> AlunosTurmas { get; set; }
        public IEnumerable<TipoNotaCicloAno> TiposNota { get; set; }
        public IEnumerable<TransferenciaDto> Transferencias { get; set; }
        public string[] TurmasCodigo { get; set; }
        public DadosDataDto DadosData { get; set; }
        public FuncionarioDto DadosDiretor { get; set; }
        public FuncionarioDto DadosSecretario { get; set; }
        public CabecalhoDto Cabecalho { get; set; }
        public LegendaDto Legenda { get; set; }
        public bool PreencherDataImpressao { get; set; }
        public bool ImprimirDadosResponsaveis { get; set; }
        public IEnumerable<FiltroHistoricoEscolarAlunosDto> FiltroHistoricoAlunos { get; set; }
    }
}
