using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class SecaoContraturnoEncaminhamentoNaapa : SecaoTabelaEncaminhamentoNaapa
    {
        public SecaoContraturnoEncaminhamentoNaapa(SecaoQuestoesEncaminhamentoNAAPADetalhadoDto informacoes) : base(informacoes)
        {
            AtividadeContraTurnos = Questao?.AtividadeContraTurnos?.ToList() ?? new List<AtividadeContraTurnoDto>();
            Tabela = AtividadeContraTurnos;
        }

        public SecaoContraturnoEncaminhamentoNaapa()
        {
            AtividadeContraTurnos = new List<AtividadeContraTurnoDto>();
        }

        public List<AtividadeContraTurnoDto> AtividadeContraTurnos { get; set; }

        protected override string NomeComponente => NomeComponentesEncaminhamentoNaapa.ATIVIDADES_CONTRATURNO;

        public override int ObterLinhasDeQuebra()
        {
            return ObterLinhaCabecalho() + AtividadeContraTurnos.Count();
        }

        public override bool PodeAdicionarLinha()
        {
            return AtividadeContraTurnos != null && AtividadeContraTurnos.Any();
        }

        protected override void AdicioneItemTabela(object item)
        {
            AtividadeContraTurnos.Add((AtividadeContraTurnoDto)item);
        }

        protected override SecaoTabelaEncaminhamentoNaapa ObterTabelaEncaminhamento()
        {
            return new SecaoContraturnoEncaminhamentoNaapa();
        }

        protected override int ObterLinhaDeQuebra(object item)
        {
            return 0;
        }
    }
}
