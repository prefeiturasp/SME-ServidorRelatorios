using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class ItineranciaAlunoDto
    {
        public ItineranciaAlunoDto()
        {
            Questoes = new List<ItineranciaQuestaoDto>();
        }

        public long Id { get; set; }
        public long ItineranciaId { get; set; }
        public string AlunoCodigo { get; set; }

        public List<ItineranciaQuestaoDto> Questoes { get; set; }
    }
}
