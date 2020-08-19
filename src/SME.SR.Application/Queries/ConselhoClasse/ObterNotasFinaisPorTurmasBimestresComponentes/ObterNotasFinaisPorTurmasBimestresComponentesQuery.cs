using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterNotasFinaisPorTurmasBimestresComponentesQuery : IRequest<IEnumerable<RetornoNotaConceitoBimestreComponenteDto>>
    {
        public ObterNotasFinaisPorTurmasBimestresComponentesQuery(long[] turmasId, string[] dresCodigos, string[] uesCodigos, int? semestre, int modalidade, string[] anos, int anoLetivo, int[] bimestres, long[] componentesCurricularesCodigos)
        {
            TurmasId = turmasId;
            DresCodigos = dresCodigos;
            UesCodigos = uesCodigos;
            Semestre = semestre;
            Modalidade = modalidade;
            Anos = anos;
            AnoLetivo = anoLetivo;
            Bimestres = bimestres;
            ComponentesCurricularesCodigos = componentesCurricularesCodigos;
        }

        public long[] TurmasId { get; set; }
        public string[] DresCodigos { get; set; }
        public string[] UesCodigos { get; set; }
        public int? Semestre { get; set; }
        public int Modalidade { get; set; }
        public string[] Anos { get; set; }
        public int AnoLetivo { get; set; }
        public int[] Bimestres { get; set; }
        public long[] ComponentesCurricularesCodigos { get; set; }
    }
}
