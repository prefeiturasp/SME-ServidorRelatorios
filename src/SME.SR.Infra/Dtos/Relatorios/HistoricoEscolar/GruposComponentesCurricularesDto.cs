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

                    if (componentes.Any(c => (!c.Nota || (c.Nota && !string.IsNullOrEmpty(c.NotaConceitoPrimeiroAno))) ||
                                             (!c.Nota || (c.Nota && !string.IsNullOrEmpty(c.NotaConceitoSegundoAno))) ||
                                             (!c.Nota || (c.Nota && !string.IsNullOrEmpty(c.NotaConceitoTerceiroAno))) ||
                                             (!c.Nota || (c.Nota && !string.IsNullOrEmpty(c.NotaConceitoQuartoAno))) ||
                                             (!c.Nota || (c.Nota && !string.IsNullOrEmpty(c.NotaConceitoQuintoAno))) ||
                                             (!c.Nota || (c.Nota && !string.IsNullOrEmpty(c.NotaConceitoSextoAno))) ||
                                             (!c.Nota || (c.Nota && !string.IsNullOrEmpty(c.NotaConceitoSetimoAno))) ||
                                             (!c.Nota || (c.Nota && !string.IsNullOrEmpty(c.NotaConceitoOitavoAno))) ||
                                             (!c.Nota || (c.Nota && !string.IsNullOrEmpty(c.NotaConceitoNonoAno)))))
                        return true;
                }

                return false;
            }
        }

    }
}
