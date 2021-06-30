using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class MontarRelatorioAcompanhamentoFechamentoQuery : IRequest<RelatorioAcompanhamentoFechamentoPorUeDto>
    {
        public MontarRelatorioAcompanhamentoFechamentoQuery(Dre dre, Ue ue, string turmaCodigo, IEnumerable<Turma> turmas, int[] bimestres, IEnumerable<FechamentoConsolidadoComponenteTurmaDto> consolidadoFechamento, IEnumerable<ConselhoClasseConsolidadoTurmaAlunoDto> consolidadoConselhosClasse, IEnumerable<PendenciaParaFechamentoConsolidadoDto> pendencias, Usuario usuario)
        {
            Dre = dre;
            Ue = ue;
            TurmaCodigo = turmaCodigo;
            Turmas = turmas;
            Bimestres = bimestres;
            ConsolidadoFechamento = consolidadoFechamento;
            ConsolidadoConselhosClasse = consolidadoConselhosClasse;
            Pendencias = pendencias;
            Usuario = usuario;
        }

        public Dre Dre { get; }
        public Ue Ue { get; }
        public IEnumerable<Turma> Turmas { get; }
        public int[] Bimestres { get; }
        public IEnumerable<FechamentoConsolidadoComponenteTurmaDto> ConsolidadoFechamento { get; }
        public IEnumerable<ConselhoClasseConsolidadoTurmaAlunoDto> ConsolidadoConselhosClasse { get; }
        public IEnumerable<PendenciaParaFechamentoConsolidadoDto> Pendencias { get; }
        public string TurmaCodigo { get; }
        public Usuario Usuario { get; }
    }
}
