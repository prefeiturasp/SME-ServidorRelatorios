using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class SecaoContatoResponsavelEncaminhamentoNaapa : SecaoTabelaEncaminhamentoNaapa
    {
        private const int COLUNA_NOME = 54;

        public SecaoContatoResponsavelEncaminhamentoNaapa(SecaoQuestoesEncaminhamentoNAAPADetalhadoDto informacoes) : base(informacoes)
        {
            ContatoResponsaveis = Questao?.ContatoResponsaveis?.ToList() ?? new List<ContatoResponsaveisDto>();
            Tabela = ContatoResponsaveis;
        }

        public SecaoContatoResponsavelEncaminhamentoNaapa() 
        {
            ContatoResponsaveis = new List<ContatoResponsaveisDto>();
        }

        public List<ContatoResponsaveisDto> ContatoResponsaveis { get; set; }

        protected override string NomeComponente => NomeComponentesEncaminhamentoNaapa.CONTATO_RESPONSAVEIS;

        public override int ObterLinhasDeQuebra()
        {
            return ObterLinhaCabecalho() + ContatoResponsaveis.Count() + ObterTotalQuebraPorColuna();
        }

        public override bool PodeAdicionarLinha()
        {
            return ContatoResponsaveis != null && ContatoResponsaveis.Any();
        }

        protected override void AdicioneItemTabela(object item)
        {
            ContatoResponsaveis.Add((ContatoResponsaveisDto)item);
        }

        protected override SecaoTabelaEncaminhamentoNaapa ObterTabelaEncaminhamento()
        {
            return new SecaoContatoResponsavelEncaminhamentoNaapa();
        }

        protected override int ObterLinhaDeQuebra(object item)
        {
            var contato = (ContatoResponsaveisDto)item;

            return contato.NomeCompleto.Length > COLUNA_NOME ? 1 : 0;
        }

        private int ObterTotalQuebraPorColuna()
        {
            var linhas = 0;

            foreach(var contato in ContatoResponsaveis)
            {
                linhas += ObterLinhaDeQuebra(contato);
            }

            return linhas;
        }
    }
}
