using System;

namespace SME.SR.Infra
{
    public class AulaReduzidaDto
    {
        public AulaReduzidaDto()
        {
        }

        public DateTime Data { get; set; }
        public int Quantidade { get; set; }
        public string Professor { get; set; }
        public string ProfessorRf { get; set; }
        public string TurmaCodigo { get; set; }
        public string ComponenteCurricularId { get; set; }
        public bool FrequenciaRegistrada { get; set; }
        public bool ControlaFrequencia { get; set; }
        public bool AulaDada()
        {
            if (!ControlaFrequencia)
                return Data.Date < DateTime.Now.Date;
            else
                return FrequenciaRegistrada;
        }
    }
}
