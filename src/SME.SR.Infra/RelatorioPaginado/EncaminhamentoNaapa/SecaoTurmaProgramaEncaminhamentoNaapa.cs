using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class SecaoTurmaProgramaEncaminhamentoNaapa : SecaoTabelaEncaminhamentoNaapa
    {
        private const int COLUNA_DRE_UE = 55;
        private const int COLUNA_COMPONENTE = 26;

        public SecaoTurmaProgramaEncaminhamentoNaapa(SecaoQuestoesEncaminhamentoNAAPADetalhadoDto informacoes) : base(informacoes)
        {
            TurmasPrograma = Questao?.TurmasPrograma?.ToList() ?? new List<TurmaProgramaDto>();
            Tabela = TurmasPrograma;
        }

        public SecaoTurmaProgramaEncaminhamentoNaapa()
        {
            TurmasPrograma = new List<TurmaProgramaDto>();
        }

        public List<TurmaProgramaDto> TurmasPrograma { get; set; }

        protected override string NomeComponente => NomeComponentesEncaminhamentoNaapa.TURMAS_PROGRAMA;

        public override int ObterLinhasDeQuebra()
        {
            return ObterLinhaCabecalho() + TurmasPrograma.Count() + ObterTotalQuebraPorColuna();
        }

        public override bool PodeAdicionarLinha()
        {
            return TurmasPrograma != null && TurmasPrograma.Any();
        }

        protected override void AdicioneItemTabela(object item)
        {
            TurmasPrograma.Add((TurmaProgramaDto)item);
        }

        protected override SecaoTabelaEncaminhamentoNaapa ObterTabelaEncaminhamento()
        {
            return new SecaoTurmaProgramaEncaminhamentoNaapa();
        }

        protected override int ObterLinhaDeQuebra(object item)
        {
            var turma = (TurmaProgramaDto)item;

            return turma.DreUe.Length > COLUNA_DRE_UE || turma.ComponenteCurricular.Length > COLUNA_COMPONENTE ? 1 : 0;
        }

        private int ObterTotalQuebraPorColuna()
        {
            var linhas = 0;

            foreach(var turma in TurmasPrograma)
            {
                linhas += ObterLinhaDeQuebra(turma);
            }

            return linhas;
        }
    }
}
