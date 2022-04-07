using SME.SR.Infra.RelatorioPaginado.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SME.SR.Infra.RelatorioPaginado
{
    public class Coluna<T> : IColuna where T : class
    {
        public bool Chave { get; set; }

        public string Nome { get; set; }

        public Func<T, object> Propriedade { get; set; }

        public string Titulo { get; set; }

        public int Largura { get; set; }

        public EnumUnidadeDeTamanho UnidadeDeTamanho { get; set; }

        public List<string> ListaDeAtributos { get; set; }

        public string ObtenhaValorDaPropriedade(object objeto)
        {
            if (Propriedade != null)
            {
                return Propriedade((T)objeto).ToString();
            }

            return string.Empty;
        }
        public string ObtenhaLarguraComUnidade() {
            var unidade = UnidadeDeTamanho == EnumUnidadeDeTamanho.PERCENTUAL ? "%" : "px";

            return Largura.ToString() + unidade;
        }

    }
}
