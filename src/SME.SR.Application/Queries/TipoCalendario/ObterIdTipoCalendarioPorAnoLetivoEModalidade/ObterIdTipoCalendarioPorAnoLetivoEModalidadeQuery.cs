using MediatR;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterIdTipoCalendarioPorAnoLetivoEModalidadeQuery: IRequest<long>
    {
        public ObterIdTipoCalendarioPorAnoLetivoEModalidadeQuery(int anoLetivo, ModalidadeTipoCalendario modalidade, int semestre)
        {
            AnoLetivo = anoLetivo;
            Modalidade = modalidade;
            Semestre = semestre;
        }

        public int AnoLetivo { get; set; }
        public ModalidadeTipoCalendario Modalidade { get; set; }
        public int Semestre { get; set; }
    }
}
