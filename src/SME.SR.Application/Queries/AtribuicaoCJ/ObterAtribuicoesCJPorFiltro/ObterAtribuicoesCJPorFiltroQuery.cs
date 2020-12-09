using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterAtribuicoesCJPorFiltroQuery : IRequest<IEnumerable<AtribuicaoCJ>>
    {
        public Modalidade Modalidade { get; set; }

        public string TurmaId { get; set; }

        public string UeId { get; set; }

        public long ComponenteCurricularId { get; set; }

        public string UsuarioRf { get; set; }

        public string UsuarioNome { get; set; }

        public bool Substituir { get; set; }

        public string DreCodigo { get; set; }

        public string[] TurmasId { get; set; }

        public long[] ComponentesCurricularesId { get; set; }

        public int AnoLetivo { get; set; }

        public int? Semestre { get; set; }
    }
}
