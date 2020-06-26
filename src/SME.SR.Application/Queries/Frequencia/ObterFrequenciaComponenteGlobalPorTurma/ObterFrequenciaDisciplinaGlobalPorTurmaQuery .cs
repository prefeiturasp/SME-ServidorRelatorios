using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Application
{
    public class ObterFrequenciaComponenteGlobalPorTurmaQuery : IRequest<IEnumerable<FrequenciaAluno>>
    {
        public ObterFrequenciaComponenteGlobalPorTurmaQuery (string turmaCodigo, ModalidadeTipoCalendario modalidade, int anoLetivo, int semestre)
        {
            TurmaCodigo = turmaCodigo;
            Modalidade = modalidade;
            AnoLetivo = anoLetivo;
            Semestre = semestre;
        }

        public string TurmaCodigo { get; set; }
        public ModalidadeTipoCalendario Modalidade { get; set; }
        public int AnoLetivo { get; set; }
        public int Semestre { get; set; }
    }
}
