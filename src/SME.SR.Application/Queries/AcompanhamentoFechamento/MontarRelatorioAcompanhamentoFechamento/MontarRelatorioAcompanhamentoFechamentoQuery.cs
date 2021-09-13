using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class MontarRelatorioAcompanhamentoFechamentoQuery : IRequest<RelatorioAcompanhamentoFechamentoPorUeDto>
    {
        public MontarRelatorioAcompanhamentoFechamentoQuery(Dre dre, Ue ue, string[] turmasCodigo, IEnumerable<Turma> turmas, IEnumerable<ComponenteCurricularPorTurma> componentesCurriculares, int[] bimestres, IEnumerable<FechamentoConsolidadoComponenteTurmaDto> consolidadoFechamento, IEnumerable<ConselhoClasseConsolidadoTurmaAlunoDto> consolidadoConselhosClasse, bool listarPendencias, IEnumerable<PendenciaParaFechamentoConsolidadoDto> pendencias, Usuario usuario, bool todasUes, IEnumerable<FechamentoConsolidadoComponenteTurmaDto> consolidadoFechamentoTodasUes, IEnumerable<ConselhoClasseConsolidadoTurmaAlunoDto> consolidadoConselhosClasseTodasUes)
        {
            Dre = dre;
            Ue = ue;
            TurmasCodigo = turmasCodigo;
            Turmas = turmas;
            Bimestres = bimestres;
            ConsolidadoFechamento = consolidadoFechamento;
            ConsolidadoConselhosClasse = consolidadoConselhosClasse;
            ComponentesCurriculares = componentesCurriculares;
            Pendencias = pendencias;
            ListarPendencias = listarPendencias;
            Usuario = usuario;
            TodasUes = todasUes;
            ConsolidadoFechamentoTodasUes = consolidadoFechamentoTodasUes;
            ConsolidadoConselhosClasseTodasUes = consolidadoConselhosClasseTodasUes;
        }

        public Dre Dre { get; set; }
        public Ue Ue { get; set; }
        public IEnumerable<Turma> Turmas { get; set; }
        public int[] Bimestres { get; set; }
        public IEnumerable<FechamentoConsolidadoComponenteTurmaDto> ConsolidadoFechamento { get; set; }
        public IEnumerable<ConselhoClasseConsolidadoTurmaAlunoDto> ConsolidadoConselhosClasse { get; set; }
        public IEnumerable<FechamentoConsolidadoComponenteTurmaDto> ConsolidadoFechamentoTodasUes { get; set; }
        public IEnumerable<ConselhoClasseConsolidadoTurmaAlunoDto> ConsolidadoConselhosClasseTodasUes { get; set; }
        public IEnumerable<ComponenteCurricularPorTurma> ComponentesCurriculares { get; set; }
        public IEnumerable<PendenciaParaFechamentoConsolidadoDto> Pendencias { get; set; }
        public bool ListarPendencias { get; set; }
        public string[] TurmasCodigo { get; set; }
        public Usuario Usuario { get; set; }
        public bool TodasUes { get; set; }
    }
}
