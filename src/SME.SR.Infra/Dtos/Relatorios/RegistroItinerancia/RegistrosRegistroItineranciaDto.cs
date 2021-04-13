using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RegistrosRegistroItineranciaDto
    {
        public string Dre { get; set; }
        public string Ue { get; set; }
        public string DataVisita { get; set; }
        public IEnumerable<ObjetivosRegistroItineranciaDto> Objetivos { get; set; }
        public IEnumerable<AlunoRegistroItineranciaDto> Alunos { get; set; }
        public string AcompanhamentoSituacao { get; set; }
        public string Encaminhamentos { get; set; }
        public string DataRetorno { get; set; }

    }
}
