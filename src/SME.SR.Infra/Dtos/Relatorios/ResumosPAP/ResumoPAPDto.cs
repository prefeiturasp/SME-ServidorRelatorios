using System.Collections.Generic;

namespace SME.SR.Infra
{
   public class ResumoPAPDto
    {
        public string DreNome { get; set; }

        public string UeNome { get; set; }

        public int AnoLetivo { get; set; }

        public string Ciclo { get; set; }

        public int Ano { get; set; }

        public string Turma { get; set; }

        public string Periodo { get; set; }

        public string UsuarioNome { get; set; }

        public string UsuarioRF { get; set; }

        public string Data { get; set; }

        public bool EhEncaminhamento { get; set; }

        public ResumoPAPTotalEstudantesDto TotalEstudantesDto { get; set; }

        public List<ResumoPAPTotalEstudanteFrequenciaDto> FrequenciaDto { get; set; }

        public List<ResumoPAPTotalResultadoDto> ResultadoDto { get; set; }

        public List<ResumoPAPTotalResultadoDto> EncaminhamentoDto { get; set; }


    }
}
