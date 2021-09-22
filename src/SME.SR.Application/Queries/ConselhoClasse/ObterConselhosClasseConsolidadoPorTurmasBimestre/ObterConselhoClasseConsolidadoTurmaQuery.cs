using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterConselhoClasseConsolidadoTurmaQuery : IRequest<IEnumerable<ConselhoClasseConsolidadoTurmaDto>>
    {
        public string DreCodigo { get; internal set; }
        public int ModalidadeCodigo { get; internal set; }
        public int[] Bimestres { get; set; }
        public SituacaoConselhoClasse? SituacaoConselhoClasse { get; set; }
        public int AnoLetivo { get; set; }
        public int Semestre { get; set; }
        public ObterConselhoClasseConsolidadoTurmaQuery(string dreCodigo, int modalidadeCodigo, int[] bimestres, SituacaoConselhoClasse? situacaoConselhoClasse, int anoLetivo, int semestre = 0)
        {
            DreCodigo = dreCodigo;
            ModalidadeCodigo = modalidadeCodigo;
            Bimestres = bimestres;
            SituacaoConselhoClasse = situacaoConselhoClasse;
            AnoLetivo = anoLetivo;
            Semestre = semestre;
        }
    }
}
