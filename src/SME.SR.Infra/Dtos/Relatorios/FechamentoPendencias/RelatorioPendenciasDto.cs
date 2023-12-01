using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioPendenciasDto
    {
        public RelatorioPendenciasDto()
        {
            
        }
        public bool ExibeDetalhamento { get; set; }        
        public string DreNome { get; set; }
        public string UeNome{ get; set; }
        public string Ano{ get; set; }
        public string Bimestre{ get; set; }
        public string ComponenteCurricular{ get; set; }
        public string Usuario{ get; set; }
        public string RF{ get; set; }
        public string Data{ get; set; }
        public string TurmaNome { get; set; }
        public string Modalidade { get; set; }
        public string Semestre { get; set; }
        public string UsuarioLogadoNome { get; set; }
        public string UsuarioLogadoRf { get; set; }
        public RelatorioPendenciasDreDto Dre { get; set; }
        public bool EhSemestral {  get; set; }
    }
}
