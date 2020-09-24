using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class GruposComponentesCurricularesDto
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }
        
        [JsonProperty("areasDeConhecimento")]
        public List<AreaDeConhecimentoDto> AreasDeConhecimento { get; set; }

        [JsonIgnore]
        public bool PossuiNotaValida
        {
            get
            {
                if (AreasDeConhecimento != null || !AreasDeConhecimento.Any())
                {
                    var componentes = AreasDeConhecimento.SelectMany(ac => ac.ComponentesCurriculares);
                    if (componentes == null || !componentes.Any())
                        return false;

                    if (componentes.Any(c => c.Nota && (!string.IsNullOrEmpty(c.NotaConceitoPrimeiroAno) ||
                                             !string.IsNullOrEmpty(c.NotaConceitoSegundoAno) ||
                                             !string.IsNullOrEmpty(c.NotaConceitoTerceiroAno) ||
                                             !string.IsNullOrEmpty(c.NotaConceitoQuartoAno) ||
                                             !string.IsNullOrEmpty(c.NotaConceitoQuintoAno) ||
                                             !string.IsNullOrEmpty(c.NotaConceitoSextoAno) ||
                                             !string.IsNullOrEmpty(c.NotaConceitoSetimoAno) ||
                                             !string.IsNullOrEmpty(c.NotaConceitoOitavoAno) ||
                                             !string.IsNullOrEmpty(c.NotaConceitoNonoAno))))
                        return true;
                }

                return false;
            }
        }

    }
}
