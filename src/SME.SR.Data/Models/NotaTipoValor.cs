using SME.SR.Infra;
using System;

namespace SME.SR.Data
{
    public class NotaTipoValor
    {
        public bool Ativo { get; set; }
        public string Descricao { get; set; }
        public DateTime FimVigencia { get; set; }
        public DateTime InicioVigencia { get; set; }
        public TipoNota TipoNota { get; set; }

        public bool EhNota()
        {
            return TipoNota == TipoNota.Nota;
        }
    }
}
