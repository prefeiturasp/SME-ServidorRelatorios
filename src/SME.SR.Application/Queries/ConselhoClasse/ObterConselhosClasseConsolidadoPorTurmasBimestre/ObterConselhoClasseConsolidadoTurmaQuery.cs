using MediatR;
using SME.SR.Infra;
using System.Collections.Generic;

namespace SME.SR.Application
{
    public class ObterConselhoClasseConsolidadoTurmaQuery : IRequest<IEnumerable<ConselhoClasseConsolidadoTurmaDto>>
    {
        public string[] TurmasCodigo { get; internal set; }
        public int ModalidadeCodigo { get; internal set; }

        public ObterConselhoClasseConsolidadoTurmaQuery(string[] turmasCodigo, int modalidadeCodigo)
        {
            TurmasCodigo = turmasCodigo;
            ModalidadeCodigo = modalidadeCodigo;
        }
    }
}
