using System;

namespace SME.SR.Infra
{
    public class HistoricoReinicioSenhaDto
    {
        public string Login { get; set; }
        public string Nome { get; set; }
        public string Perfil { get; set; }
        public string SenhaReiniciada { get; set; }
        public string SenhaReiniciadaPor { get; set; }
        public string UtilizaSenhaPadao { get; set; }
    }
}
