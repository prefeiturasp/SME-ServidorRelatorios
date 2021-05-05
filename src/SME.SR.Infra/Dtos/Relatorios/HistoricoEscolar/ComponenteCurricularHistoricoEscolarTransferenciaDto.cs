using Newtonsoft.Json;

namespace SME.SR.Infra
{
    public class ComponenteCurricularHistoricoEscolarTransferenciaDto
    {
        [JsonIgnore]
        public string Codigo { get; set; }

        [JsonIgnore]
        public bool Nota { get; set; }

        [JsonIgnore]
        public bool Frequencia { get; set; }

        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("notaConceitoPrimeiroBimestre")]
        public string NotaConceitoPrimeiroBimestre { get; set; }

        [JsonProperty("frequenciaPrimeiroBimestre")]
        public string FrequenciaPrimeiroBimestre { get; set; }

        [JsonProperty("notaConceitoSegundoBimestre")]
        public string NotaConceitoSegundoBimestre { get; set; }

        [JsonProperty("frequenciaSegundoBimestre")]
        public string FrequenciaSegundoBimestre { get; set; }

        [JsonProperty("notaConceitoTerceiroBimestre")]
        public string NotaConceitoTerceiroBimestre { get; set; }

        [JsonProperty("frequenciaTerceiroBimestre")]
        public string FrequenciaTerceiroBimestre { get; set; }

        [JsonProperty("notaConceitoQuartoBimestre")]
        public string NotaConceitoQuartoBimestre { get; set; }

        [JsonProperty("frequenciaQuartoBimestre")]
        public string FrequenciaQuartoBimestre { get; set; }

        [JsonIgnore]
        public bool PossuiNotaValida
        {
            get
            {
                if (Nota && (!string.IsNullOrEmpty(NotaConceitoPrimeiroBimestre) ||
                    !string.IsNullOrEmpty(NotaConceitoSegundoBimestre) ||
                    !string.IsNullOrEmpty(NotaConceitoTerceiroBimestre) ||
                    !string.IsNullOrEmpty(NotaConceitoQuartoBimestre)))
                    return true;
                else if (!Nota && (!string.IsNullOrEmpty(FrequenciaPrimeiroBimestre) ||
                   !string.IsNullOrEmpty(FrequenciaSegundoBimestre) ||
                   !string.IsNullOrEmpty(FrequenciaTerceiroBimestre) ||
                   !string.IsNullOrEmpty(FrequenciaQuartoBimestre)))
                    return true;

                return false;
            }
        }


    }
}
