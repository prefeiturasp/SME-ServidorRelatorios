using System;

namespace SME.SR.Infra.Dtos.Relatorios.CDEP
{
    public class AcervoDevolucaoDto
    {
        public string Solicitante { get; set; }
        public string Tombo { get; set; }
        public string Titulo { get; set; }
        public string Telefone { get; set; }
        public string Email { get; set; }
        public DateTime DataEmprestimo { get; set; }
        public DateTime DataDevolucao { get; set; }
        public int DiasEmprestimo
        {
            get
            {
                TimeSpan duracao = DataDevolucao - DataEmprestimo;
                return duracao.Days;
            }
        }
    }
}
