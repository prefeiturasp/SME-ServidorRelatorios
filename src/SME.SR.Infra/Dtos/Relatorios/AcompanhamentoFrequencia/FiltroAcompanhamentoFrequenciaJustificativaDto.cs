using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class FiltroAcompanhamentoFrequenciaJustificativaDto
    {
        public string DreCodigo { get; set; }
        public string UeCodigo { get; set; }
        public string TurmaCodigo { get; set; }
        public string Bimestre { get; set; }
        public string UsuarioRF { get; set; }
        public string UsuarioNome { get; set; }
        public string ComponenteCurricularId { get; set; }
        public List<string> AlunosCodigos { get; set; }
        public bool ImprimirFrequenciaDiaria { get; set; }
    }
}
