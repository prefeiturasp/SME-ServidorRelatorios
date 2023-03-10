using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class SecaoEnderecoEncaminhamentoNaapa : SecaoTabelaEncaminhamentoNaapa
    {
        private const int COLUNA_LOGRADOURO = 32;
        private const int COLUNA_COMPLEMENTO = 20;
        private const int COLUNA_BAIRRO = 20;

        public SecaoEnderecoEncaminhamentoNaapa(SecaoQuestoesEncaminhamentoNAAPADetalhadoDto informacoes) : base(informacoes)
        {
            Enderecos = Questao?.Enderecos?.ToList() ?? new List<EnderecoDto>();
            Tabela = Enderecos;
        }

        public SecaoEnderecoEncaminhamentoNaapa() 
        {
            Enderecos = new List<EnderecoDto>();
        }

        public List<EnderecoDto> Enderecos { get; set; }

        protected override string NomeComponente => NomeComponentesEncaminhamentoNaapa.ENDERECO_RESIDENCIAL;

        public override int ObterLinhasDeQuebra()
        {
            return ObterLinhaCabecalho() + Enderecos.Count() + ObterTotalQuebraPorColuna();
        }

        public override bool PodeAdicionarLinha()
        {
            return Enderecos != null && Enderecos.Any();
        }

        protected override void AdicioneItemTabela(object item)
        {
            Enderecos.Add((EnderecoDto)item);
        }

        protected override SecaoTabelaEncaminhamentoNaapa ObterTabelaEncaminhamento()
        {
            return new SecaoEnderecoEncaminhamentoNaapa();
        }

        protected override int ObterLinhaDeQuebra(object item)
        {
            var endereco = (EnderecoDto)item;

            return endereco.Logradouro.Length > COLUNA_LOGRADOURO ||
                endereco.Complemento.Length > COLUNA_COMPLEMENTO ||
                endereco.Bairro.Length > COLUNA_BAIRRO ? 1 : 0;
        }

        private int ObterTotalQuebraPorColuna()
        {
            var linhas = 0;
            
            foreach (var endereco in Enderecos)
            {
                linhas += ObterLinhaDeQuebra(endereco);
            }

            return linhas;
        }
    }
}
