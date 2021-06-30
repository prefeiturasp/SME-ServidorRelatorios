using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class MontarRelatorioAcompanhamentoFechamentoQuery : IRequest<RelatorioAcompanhamentoFechamentoPorUeDto>
    {
        public MontarRelatorioAcompanhamentoFechamentoQuery(Dre dre, Ue ue, string turmaCodigo, IEnumerable<Turma> turmas, IEnumerable<DisciplinaDto> componentesCurriculares, int[] bimestres, IEnumerable<FechamentoConsolidadoComponenteTurmaDto> consolidadoFechamento, IEnumerable<ConselhoClasseConsolidadoTurmaAlunoDto> consolidadoConselhosClasse, bool listarPendencias, IEnumerable<PendenciaParaFechamentoConsolidadoDto> pendencias, Usuario usuario)
        {
            Dre = dre;
            Ue = ue;
            TurmaCodigo = turmaCodigo;
            Turmas = turmas;
            Bimestres = bimestres;
            ConsolidadoFechamento = consolidadoFechamento;
            ConsolidadoConselhosClasse = consolidadoConselhosClasse;
            ComponentesCurriculares = componentesCurriculares;
            Pendencias = pendencias;
            ListarPendencias = listarPendencias;
            Usuario = usuario;
        }

        public Dre Dre { get; set; }
        public Ue Ue { get; set; }
        public IEnumerable<Turma> Turmas { get; set; }
        public int[] Bimestres { get; set; }
        public IEnumerable<FechamentoConsolidadoComponenteTurmaDto> ConsolidadoFechamento { get; set; }
        public IEnumerable<ConselhoClasseConsolidadoTurmaAlunoDto> ConsolidadoConselhosClasse { get; set; }
        public IEnumerable<DisciplinaDto> ComponentesCurriculares { get; set; }
        public IEnumerable<PendenciaParaFechamentoConsolidadoDto> Pendencias { get; set; }
        public bool ListarPendencias { get; set; }
        public string TurmaCodigo { get; set; }
        public Usuario Usuario { get; set; }
    }
}
