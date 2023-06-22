using System.Collections;
using System.Collections.Generic;
using MediatR;
using SME.SR.Infra.Dtos.FrequenciaMensal;

namespace SME.SR.Application
{
    public class ObterFrequenciaRealatorioControleMensalQuery :IRequest<IEnumerable<ConsultaRelatorioFrequenciaControleMensalDto>>
    {
        public ObterFrequenciaRealatorioControleMensalQuery(int anoLetivo, string[] mes, string ueCodigo, string dreCodigo, int modalidade, int semestre, string turmaCodigo, string[] alunosCodigo)
        {
            AnoLetivo = anoLetivo;
            Mes = mes;
            UeCodigo = ueCodigo;
            DreCodigo = dreCodigo;
            Modalidade = modalidade;
            Semestre = semestre;
            TurmaCodigo = turmaCodigo;
            AlunosCodigo = alunosCodigo;
        }

        public int AnoLetivo { get; set; }
        public string[] Mes { get; set; }
        public string UeCodigo { get; set; }
        public string DreCodigo { get; set; }
        public int Modalidade { get; set; }
        public int Semestre { get; set; }
        public string TurmaCodigo { get; set; }
        public string[] AlunosCodigo { get; set; }
    }
}