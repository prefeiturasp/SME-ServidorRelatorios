using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SME.SR.Infra
{
    public class BaseNacionalComumDto
    {
        [JsonProperty("areasDeConhecimento")] public List<AreaDeConhecimentoDto> AreasDeConhecimento { get; set; }

        [JsonIgnore]
        public BaseNacionalComumDto ObterComNotaValida
        {
            get
            {
                if (AreasDeConhecimento != null || !AreasDeConhecimento.Any())
                {
                    var componentes = AreasDeConhecimento.SelectMany(ac => ac.ComponentesCurriculares);
                    if (componentes == null || !componentes.Any())
                        return null;

                    if (componentes.Any(c => c.Nota && (!string.IsNullOrEmpty(c.NotaConceitoPrimeiroAno) ||
                                             !string.IsNullOrEmpty(c.NotaConceitoSegundoAno) ||
                                             !string.IsNullOrEmpty(c.NotaConceitoTerceiroAno) ||
                                             !string.IsNullOrEmpty(c.NotaConceitoQuartoAno) ||
                                             !string.IsNullOrEmpty(c.NotaConceitoQuintoAno) ||
                                             !string.IsNullOrEmpty(c.NotaConceitoSextoAno) ||
                                             !string.IsNullOrEmpty(c.NotaConceitoSetimoAno) ||
                                             !string.IsNullOrEmpty(c.NotaConceitoOitavoAno) ||
                                             !string.IsNullOrEmpty(c.NotaConceitoNonoAno))))
                        return this;
                }

                return null;
            }
        }
    }
}