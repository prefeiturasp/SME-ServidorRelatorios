using SME.SR.Infra.Utilitarios;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class LeituraComunicadoTurmaDto
    {
        public LeituraComunicadoTurmaDto()
        {
            LeituraComunicadoEstudantes = new List<LeituraComunicadoEstudanteDto>();
        }

        public long ComunicadoId { get; set; }
        public string Turma { get => $"{TurmaNome} - {TurmaModalidade.ShortName()}"; }
        public string TurmaNome { get; set; }
        public string TurmaCodigo { get; set; }
        public Modalidade TurmaModalidade { get; set; }
        public long NaoInstalado { get; set; }
        public long NaoVisualizado { get; set; }
        public long Visualizado { get; set; }
        public List<LeituraComunicadoEstudanteDto> LeituraComunicadoEstudantes { get; set; }
    }
}
