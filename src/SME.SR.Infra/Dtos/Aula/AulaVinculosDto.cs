using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class AulaVinculosDto
    {
        public AulaVinculosDto()
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
        public bool PossuiAtividadeAvaliativa { get; set; }
        public bool PossuiNotaLancada { get; set; }
        public bool AulaDada()
        {
            if (!ControlaFrequencia)
                return Data.Date < DateTime.Now.Date;
            else
                return FrequenciaRegistrada;
        }

        public string Observacoes()
        {
            if (PossuiAtividadeAvaliativa)
                return $"Avaliação {(Data.Date < DateTime.Now.Date ? "aplicada": "não aplicada")} {(PossuiNotaLancada ? "com" : "sem")} notas lançadas.";

            return "";
        }
    }
}
