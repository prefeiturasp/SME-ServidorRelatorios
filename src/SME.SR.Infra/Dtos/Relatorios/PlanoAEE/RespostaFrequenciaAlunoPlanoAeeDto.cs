using System;

namespace SME.SR.Infra
{
    public class RespostaFrequenciaAlunoPlanoAeeDto
    {
        public string DiaSemana { get; set; }
        public DateTime HorarioInicio { get; set; }
        public DateTime HorarioTermino { get; set; }
        public long Id { get; set; }
    }
}