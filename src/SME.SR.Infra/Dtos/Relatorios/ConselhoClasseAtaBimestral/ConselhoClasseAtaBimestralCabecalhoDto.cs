using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class ConselhoClasseAtaBimestralCabecalhoDto
    {
        public string Dre { get; set; }
        public string Ue { get; set; }
        public string Turma { get; set; }
        public int AnoLetivo { get; set; }
        public string Usuario { get; set; }
        public string RF { get; set; }
        public string ModalidadeResumida { get; set; }
        public string TurmaFormatada { get => $"{ModalidadeResumida} - {Turma}"; }
        public string Bimestre { get; set; }
        public string Data => DateTime.Now.ToString("dd/MM/yyyy");
        public string TituloFormatado
        {
            get
            {
                if (!String.IsNullOrEmpty(Bimestre) && Bimestre != "0")
                {
                    return $"ATA BIMESTRAL - {Bimestre}° BIMESTRE - {AnoLetivo}";
                }
                else
                {
                    return $"ATA BIMESTRAL - {AnoLetivo}";
                }

            }
        }
    }
}
