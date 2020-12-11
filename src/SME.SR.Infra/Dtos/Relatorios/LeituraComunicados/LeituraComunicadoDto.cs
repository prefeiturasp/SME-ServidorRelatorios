using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class LeituraComunicadoDto
    {
        public LeituraComunicadoDto()
        {
            LeituraComunicadoTurma = new List<LeituraComunicadoTurmaDto>();
        }

        public long ComunicadoId { get; set; }
        public string Comunicado { get; set; }
        public DateTime DataEnvio { get; set; }
        public DateTime DataExpiracao { get; set; }
        public long NaoInstalado { get; set; }
        public long NaoVisualizado { get; set; }
        public long Visualizado { get; set; }
        public List<LeituraComunicadoTurmaDto> LeituraComunicadoTurma { get; set; }
    }
}
