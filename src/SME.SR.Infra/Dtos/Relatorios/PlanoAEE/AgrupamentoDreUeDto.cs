using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class AgrupamentoDreUeDto
    {
        public string DreNome { get; set; }
        public string UeNome { get; set; }
        public List<DetalhePlanosAeeDto> Detalhes { get; set; }

        public AgrupamentoDreUeDto()
        {
            Detalhes = new List<DetalhePlanosAeeDto>();
        } 
    }
}