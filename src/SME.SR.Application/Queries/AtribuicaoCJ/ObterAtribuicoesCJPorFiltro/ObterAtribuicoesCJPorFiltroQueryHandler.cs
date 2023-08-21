using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAtribuicoesCJPorFiltroQueryHandler : IRequestHandler<ObterAtribuicoesCJPorFiltroQuery, IEnumerable<AtribuicaoCJ>>
    {
        private readonly IAtribuicaoCJRepository atribuicaoCJRepository;

        public ObterAtribuicoesCJPorFiltroQueryHandler(IAtribuicaoEsporadicaRepository atribuicaoEsporadicaRepository, IAtribuicaoCJRepository atribuicaoCJRepository)
        {
            this.atribuicaoCJRepository = atribuicaoCJRepository;
        }

        public async Task<IEnumerable<AtribuicaoCJ>> Handle(ObterAtribuicoesCJPorFiltroQuery request, CancellationToken cancellationToken)
        {
            var lstAtribuicoes = await atribuicaoCJRepository.ObterPorFiltros(request.Modalidade,
                                                        request.TurmaId, request.UeId, request.ComponenteCurricularId, request.UsuarioRf,
                                                        request.UsuarioNome, true, request.DreCodigo, anoLetivo: request.AnoLetivo);

            return lstAtribuicoes;
        }
    }
}
