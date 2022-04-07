using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.RelatorioPaginado
{
    public sealed class TipoPapel
    {
        public static readonly TipoPapel A4 = new TipoPapel(EnumTipoDePapel.A4, 1120, 790);
        public static readonly TipoPapel A4_Retrato = new TipoPapel(EnumTipoDePapel.A4_RETRATO, 790, 1120);

        private static readonly List<TipoPapel> lista = new List<TipoPapel>() { A4, A4_Retrato };

        public EnumTipoDePapel TipoDePapel { get; private set; }

        public int AlturaPx { get; private set; }

        public int LarguraPx { get; private set; }

        public TipoPapel ObtenhaTipo(EnumTipoDePapel enumTipo)
        {
            return lista.Find(x => x.TipoDePapel == enumTipo);
        }

        private TipoPapel(EnumTipoDePapel tipo, int altura, int largura)
        {
            this.TipoDePapel = tipo;
            this.AlturaPx = altura;
            this.LarguraPx = largura;
        }
    }
}
