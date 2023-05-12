using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra.Dtos.FrequenciaMensal
{
    public class FiltroRelatorioControleFrenquenciaMensalDto
    {
        public FiltroRelatorioControleFrenquenciaMensalDto()
        {
            MesesReferencias = new List<string>();
            AlunosCodigo = new List<string>().ToArray();
        }
        public bool ExibirHistorico { get; set; }
        public int AnoLetivo { get; set; }
        public string CodigoDre { get; set; }
        public string CodigoUe { get; set; }
        public Modalidade Modalidade { get; set; }
        public string Semestre { get; set; }
        public string CodigoTurma { get; set; }
        public string[] AlunosCodigo { get; set; }
        public IList<string> MesesReferencias { get; set; }
        public TipoFormatoRelatorio TipoFormatoRelatorio { get; set; }
        public string NomeUsuario { get; set; }
        public string CodigoRf { get; set; }
    }
}