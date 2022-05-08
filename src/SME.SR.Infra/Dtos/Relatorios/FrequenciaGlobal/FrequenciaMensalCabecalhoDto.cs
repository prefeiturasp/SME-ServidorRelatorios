using System;

namespace SME.SR.Infra
{
    public class FrequenciaMensalCabecalhoDto
    {
        public int AnoLetivo { get; set; }
        public string NomeDre { get; set; }
        public string NomeUe { get; set; }
        public string NomeModalidade { get; set; }
        public string NomeTurma { get; set; }
        public string MesReferencia { get; set; }
        public string UsuarioSolicitante { get; set; }
        public string RfUsuarioSolicitante { get; set; }
        public string DataImpressao => DateTime.Now.ToString("dd/MM/yyyy");
    }
}