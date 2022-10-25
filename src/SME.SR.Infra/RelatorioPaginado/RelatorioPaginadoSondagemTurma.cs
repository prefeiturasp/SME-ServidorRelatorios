using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioPaginadoSondagemTurma : RelatorioPaginadoColuna<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto>
    {
        public RelatorioPaginadoSondagemTurma(ParametroRelatorioPaginadoPorColuna<RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto> parametro, List<IColuna> colunas) : base(parametro, colunas)
        {
        }

        protected override string ObtenhaValorDaColunaCustom(IColuna coluna, RelatorioSondagemComponentesPorTurmaPlanilhaLinhasDto valor)
        {
            var resposta = valor.OrdensRespostas.Find(resposta => resposta.PerguntaId.ToString() == coluna.Nome);

            if (resposta == null)
                return string.Empty;

            return resposta.Resposta;
        }
    }
}
