using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
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
                                                        request.UsuarioNome, request.Substituir, request.DreCodigo, request.TurmasId,
                                                        request.ComponentesCurricularesId, request.AnoLetivo);

            return lstAtribuicoes;
        }
    }
}
