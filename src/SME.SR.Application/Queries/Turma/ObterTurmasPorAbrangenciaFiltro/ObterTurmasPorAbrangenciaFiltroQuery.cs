using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterTurmasPorAbrangenciaFiltroQuery : IRequest<IEnumerable<Turma>>
    {
        public string CodigoUe { get; set; }

        public Modalidade? Modalidade { get; set; }

        public int AnoLetivo { get; set; }

        public int? Semestre { get; set; }
        public string Login { get; set; }
        public Guid Perfil { get; set; }
        public bool ConsideraHistorico { get; set; }
    }
}
