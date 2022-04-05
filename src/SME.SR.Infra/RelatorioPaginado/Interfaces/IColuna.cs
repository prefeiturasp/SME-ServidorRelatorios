using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.RelatorioPaginado.Interfaces
{
    public interface IColuna
    {
        public bool Chave { get; set; }

        public string Nome { get; set; }

        public string Titulo { get; set; }

        public int Largura { get; set; }

        public List<string> ListaDeAtributos { get; set; }

        public string ObtenhaValorDaPropriedade(object objeto);
    }
}
