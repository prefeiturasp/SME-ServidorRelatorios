using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class GruposComponentesCurricularesEJADto
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("areasDeConhecimento")]
        public List<AreaDeConhecimentoEJADto> AreasDeConhecimento { get; set; }

        [JsonIgnore]
        public GruposComponentesCurricularesEJADto ObterAreasComNotaValida
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

                    if (componentes.Any(c => c.Nota && (!string.IsNullOrEmpty(c.NotaConceitoPrimeiraEtapaCiclo1) ||
                                              !string.IsNullOrEmpty(c.NotaConceitoPrimeiraEtapaCiclo2) ||
                                              !string.IsNullOrEmpty(c.NotaConceitoPrimeiraEtapaCiclo3) ||
                                              !string.IsNullOrEmpty(c.NotaConceitoPrimeiraEtapaCiclo4) ||
                                              !string.IsNullOrEmpty(c.NotaConceitoSegundaEtapaCiclo1) ||
                                              !string.IsNullOrEmpty(c.NotaConceitoSegundaEtapaCiclo2) ||
                                              !string.IsNullOrEmpty(c.NotaConceitoSegundaEtapaCiclo3) ||
                                              !string.IsNullOrEmpty(c.NotaConceitoSegundaEtapaCiclo4))))
                        return true;
                    else if (componentes.Any(c => !c.Nota && (!string.IsNullOrEmpty(c.FrequenciaPrimeiraEtapaCiclo1) ||
                                              !string.IsNullOrEmpty(c.FrequenciaPrimeiraEtapaCiclo2) ||
                                              !string.IsNullOrEmpty(c.FrequenciaPrimeiraEtapaCiclo3) ||
                                              !string.IsNullOrEmpty(c.FrequenciaPrimeiraEtapaCiclo4) ||
                                              !string.IsNullOrEmpty(c.FrequenciaSegundaEtapaCiclo1) ||
                                              !string.IsNullOrEmpty(c.FrequenciaSegundaEtapaCiclo2) ||
                                              !string.IsNullOrEmpty(c.FrequenciaSegundaEtapaCiclo3) ||
                                              !string.IsNullOrEmpty(c.FrequenciaSegundaEtapaCiclo4))))
                        return true;
                }

                return false;
            }
        }
    }
}
