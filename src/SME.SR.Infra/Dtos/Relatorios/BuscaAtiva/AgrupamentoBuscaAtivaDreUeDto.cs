using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class AgrupamentoBuscaAtivaDreUeDto
    {
        public AgrupamentoBuscaAtivaDreUeDto()
        {
            Detalhes = new List<DetalheBuscaAtivaDto>();
        }

        public string DreCodigo { get; set; }
        public string DreNome { get; set; }
        public string UeNome { get; set; }
        public string UeOrdenacao { get; set; }
        public bool MostrarAgrupamento { get; set; }
        public List<DetalheBuscaAtivaDto> Detalhes { get; set; }
    }
}
