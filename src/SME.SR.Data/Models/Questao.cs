using System.Collections.Generic;
using SME.SR.Infra;

namespace SME.SR.Data
{
    public class Questao
    {
        public Questao()
        {
            OpcoesRespostas = new List<OpcaoResposta>();
        }

        public long Id { get; set; }
        public int Ordem { get; set; }
        public string Nome { get; set; }
        public TipoQuestao Tipo { get; set; }
        public List<OpcaoResposta> OpcoesRespostas { get; }
    }
}