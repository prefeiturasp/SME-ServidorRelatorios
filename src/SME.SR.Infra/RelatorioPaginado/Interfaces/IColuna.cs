using System.Collections.Generic;

namespace SME.SR.Infra
{
    public interface IColuna
    {
        public bool Chave { get; set; }

        public string Nome { get; set; }

        public string Titulo { get; set; }

        public int Largura { get; set; }

        public List<string> ListaDeAtributos { get; set; }

        public string ObtenhaValorDaPropriedade(object objeto);

        public string ObtenhaLarguraComUnidade();

        public string ObtenhaClasse();
    }
}
