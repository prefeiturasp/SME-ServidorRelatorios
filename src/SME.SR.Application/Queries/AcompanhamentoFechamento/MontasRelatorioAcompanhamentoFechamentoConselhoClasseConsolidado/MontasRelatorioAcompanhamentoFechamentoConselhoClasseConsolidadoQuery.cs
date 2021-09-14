using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class MontasRelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoQuery : IRequest<RelatorioAcompanhamentoFechamentoConsolidadoPorUeDto>
    {
        public MontasRelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoQuery(Dre dre, Ue ue, IEnumerable<Turma> turmas, int[] bimestres, IEnumerable<FechamentoConsolidadoTurmaDto> consolidadoFechamento, IEnumerable<ConselhoClasseConsolidadoTurmaDto> consolidadoConselhosClasse, string[] turmasCodigo, Usuario usuario)
        {
            Dre = dre;
            Ue = ue;
            Turmas = turmas;
            Bimestres = bimestres;
            ConsolidadoFechamento = consolidadoFechamento;
            ConsolidadoConselhosClasse = consolidadoConselhosClasse;
            TurmasCodigo = turmasCodigo;
            Usuario = usuario;
        }

        public Dre Dre { get; set; }
        public Ue Ue { get; set; }
        public IEnumerable<Turma> Turmas { get; set; }
        public int[] Bimestres { get; set; }
        public IEnumerable<FechamentoConsolidadoTurmaDto> ConsolidadoFechamento { get; set; }
        public IEnumerable<ConselhoClasseConsolidadoTurmaDto> ConsolidadoConselhosClasse { get; set; }
        public string[] TurmasCodigo { get; set; }
        public Usuario Usuario { get; set; }
    }
}
