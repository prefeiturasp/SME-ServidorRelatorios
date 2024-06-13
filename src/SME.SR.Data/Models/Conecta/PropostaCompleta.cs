using System.Collections.Generic;

namespace SME.SR.Data.Models.Conecta
{
    public class PropostaCompleta : Proposta
    {
        public TipoFormacao TipoFormacao { get; set; }
        public Formato Modalidade { get; set; }
        public string Justificativa { get; set; }
        public string Objetivos { get; set; }
        public string ConteudoProgramatico { get; set; }
        public string Procedimentos { get; set; }
        public string Referencias { get; set; }
        public string DescricaoAtividade { get; set; }
        public IEnumerable<PropostaLocal> Encontros { get; set; }
        public IEnumerable<PropostaPublicoAlvo> FuncaoEspecifica { get; set; }
        public IEnumerable<PropostaPublicoAlvo> VagasRemanecentes { get; set; }
        public IEnumerable<AreaPromotoraTelefone> TelefonesAreaPromotora { get; set; }
    }
}
