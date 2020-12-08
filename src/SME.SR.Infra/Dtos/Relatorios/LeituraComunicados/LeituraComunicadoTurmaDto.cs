using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class LeituraComunicadoTurmaDto
    {
        public LeituraComunicadoTurmaDto()
        {
            LeituraComunicadoEstudante = new List<LeituraComunicadoEstudanteDto>();
        }

        public string Turma { get; set; }
        public long NaoInstalado { get; set; }
        public long NaoVisualizado { get; set; }
        public long Visualizado { get; set; }
        public List<LeituraComunicadoEstudanteDto> LeituraComunicadoEstudante { get; set; }
    }
}
