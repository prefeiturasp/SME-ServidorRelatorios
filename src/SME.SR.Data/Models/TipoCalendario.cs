using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Data.Models
{
    public class TipoCalendario
    {
        public long Id { get; set; }
        public int AnoLetivo { get; set; }
        public bool Excluido { get; set; }
        public bool Migrado { get; set; }
        public ModalidadeTipoCalendario Modalidade { get; set; }
        public string Nome { get; set; }
        public Periodo Periodo { get; set; }
        public bool Situacao { get; set; }

        public int QuantidadeDeBimestres()
        {
            if (Modalidade == ModalidadeTipoCalendario.EJA && AnoLetivo > 2021)
                return 2;

            return 4;
        }

        public Modalidade ObterModalidadeTurma()
        {
            return Modalidade == ModalidadeTipoCalendario.EJA ?
                    Infra.Modalidade.EJA :
                    Modalidade == ModalidadeTipoCalendario.Infantil ?
                    Infra.Modalidade.Infantil :
                    Infra.Modalidade.Medio;
        }
    }
}
