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
        public GruposComponentesCurricularesDto ObterAreasComNotaValida
        {
            get
            {
                AreasDeConhecimento = AreasDeConhecimento.Where(ac => ac.ComponentesCurriculares.Any())?.ToList();
                return this;
            }
        }

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
                   else if (componentes.Any(c => !c.Nota && (!string.IsNullOrEmpty(c.FrequenciaPrimeiroAno) ||
                                             !string.IsNullOrEmpty(c.FrequenciaSegundoAno) ||
                                             !string.IsNullOrEmpty(c.FrequenciaTerceiroAno) ||
                                             !string.IsNullOrEmpty(c.FrequenciaQuartoAno) ||
                                             !string.IsNullOrEmpty(c.FrequenciaQuintoAno) ||
                                             !string.IsNullOrEmpty(c.FrequenciaSextoAno) ||
                                             !string.IsNullOrEmpty(c.FrequenciaSetimoAno) ||
                                             !string.IsNullOrEmpty(c.FrequenciaOitavoAno) ||
                                             !string.IsNullOrEmpty(c.FrequenciaNonoAno))))
                        return true;
                }

                return false;
            }
        }

    }
}
