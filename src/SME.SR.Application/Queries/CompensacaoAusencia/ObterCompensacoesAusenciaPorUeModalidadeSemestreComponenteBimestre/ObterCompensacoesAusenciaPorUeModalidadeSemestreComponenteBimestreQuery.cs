using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterCompensacoesAusenciaPorUeModalidadeSemestreComponenteBimestreQuery : IRequest<IEnumerable<RelatorioCompensacaoAusenciaRetornoConsulta>>
    {
        public ObterCompensacoesAusenciaPorUeModalidadeSemestreComponenteBimestreQuery(long ueId, Modalidade modalidade, int? semestre, string turmaCodigo, long[] componetesCurricularesIds, int bimestre, int anoLetivo)
        {
            UeId = ueId;
            Modalidade = modalidade;
            Semestre = semestre;
            TurmaCodigo = turmaCodigo;
            ComponetesCurricularesIds = componetesCurricularesIds;
            Bimestre = bimestre;
            AnoLetivo = anoLetivo;
        }

        public long UeId { get; set; }
        public Modalidade Modalidade { get; set; }
        public int? Semestre { get; set; }
        public string TurmaCodigo { get; set; }
        public long[] ComponetesCurricularesIds { get; set; }
        public int Bimestre { get; set; }
        public int AnoLetivo { get; set; }
    }
}
