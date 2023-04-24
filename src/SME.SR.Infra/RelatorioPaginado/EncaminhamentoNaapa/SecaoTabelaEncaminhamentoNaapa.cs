using System.Collections;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public abstract class SecaoTabelaEncaminhamentoNaapa : SecaoRelatorioEncaminhamentoNaapa
    {
        public SecaoTabelaEncaminhamentoNaapa(SecaoQuestoesEncaminhamentoNAAPADetalhadoDto secao)
        {
            Questao = secao.Questoes.Find(q => q.NomeComponente == NomeComponente);
            Titulo = Questao?.Questao?.ToUpper();
        }

        public SecaoTabelaEncaminhamentoNaapa()
        {
            Titulo = string.Empty;
        }

        public string Titulo { get; set; }
        public abstract bool PodeAdicionarLinha();
        protected QuestaoEncaminhamentoNAAPADetalhadoDto Questao { get; }
        protected abstract string NomeComponente { get; }
        protected abstract void AdicioneItemTabela(object item);
        protected abstract SecaoTabelaEncaminhamentoNaapa ObterTabelaEncaminhamento();
        protected IList Tabela {  get; set; }
        protected abstract int ObterLinhaDeQuebra(object item);

        public List<SecaoTabelaEncaminhamentoNaapa> ObterSecaoTabelaPaginada(int totalLinhas, int totalLinhaAtual)
        {
            var paginas = new List<SecaoTabelaEncaminhamentoNaapa>();
            totalLinhas -= 1;
            var totalLinhasRestante = totalLinhas - totalLinhaAtual - 1;

            if (PodeAdicionarLinha())
            {
                if (totalLinhasRestante < Tabela.Count)
                {
                    var linhaAtual = 0;
                    var pagina = 1;
                    var paginaAtual = ObterTabelaEncaminhamento();

                    paginaAtual.Titulo = Titulo;

                    foreach (var item in Tabela)
                    {
                        linhaAtual += ObterLinhaDeQuebra(item) + 1;

                        if (FimDaPagina(pagina, linhaAtual, totalLinhas, totalLinhasRestante))
                        {
                            paginas.Add(paginaAtual);
                            paginaAtual = ObterTabelaEncaminhamento();
                            pagina++;
                            linhaAtual = 0;
                        }

                        paginaAtual.AdicioneItemTabela(item);
                    }

                    paginas.Add(paginaAtual);

                }
                else
                  paginas.Add(this);
            }

            return paginas;
        }

        protected int ObterLinhaCabecalho()
        {
            return string.IsNullOrEmpty(Titulo) ? 1 : 2;
        }

        private bool FimDaPagina(int pagina, int linhaAtual, int totalLinhas, int totalLinhasRestante)
        {
            return (pagina == 1 && totalLinhasRestante > 2 && linhaAtual >= totalLinhasRestante) 
                    || (linhaAtual >= totalLinhas);
        }
    }
}
