using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterTurmaCodigosAlunoPorAnoLetivoAlunoTipoTurmaQuery : IRequest<IEnumerable<int>>
    {
        public ObterTurmaCodigosAlunoPorAnoLetivoAlunoTipoTurmaQuery(int anoLetivo, string[] codigoAlunos, IEnumerable<int> tiposTurmas, bool consideraHistorico, DateTime? dataReferencia = null, string ueCodigo = null)
        {
            AnoLetivo = anoLetivo;
            CodigoAlunos = codigoAlunos;
            TiposTurmas = tiposTurmas;
            DataReferencia = dataReferencia;
            ConsideraHistorico = consideraHistorico;
            UeCodigo = ueCodigo;
        }
        public int AnoLetivo { get; set; }
        public string[] CodigoAlunos { get; set; }
        public IEnumerable<int> TiposTurmas { get; set; }
        public DateTime? DataReferencia { get; }
        public bool ConsideraHistorico { get; set; }
        public string UeCodigo { get; set; }
    }
}
