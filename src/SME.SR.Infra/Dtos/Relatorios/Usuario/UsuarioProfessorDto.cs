using System;

namespace SME.SR.Infra
{
    public class UsuarioProfessorDto
    {
        public string Rf { get; set; }
        public string Nome { get; set; }
        public string Situacao { get; set; }
        public string UltimoAcesso { get; set; }
        public string UltimaAulaRegistrada { get; set; }
        public string UltimoPlanoAulaRegistrado { get; set; }
        public string UltimaFrequenciaRegistrada { get; set; }
    }
}
