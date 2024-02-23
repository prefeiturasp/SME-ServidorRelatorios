using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterAnexosEncaminhamentoNAAPAQueryHandler : IRequestHandler<ObterAnexosEncaminhamentoNAAPAQuery, IEnumerable<ArquivoDto>>
    {
        private readonly IEncaminhamentoNAAPARespostaRepository repositorio;

        public ObterAnexosEncaminhamentoNAAPAQueryHandler(IEncaminhamentoNAAPARespostaRepository repositorio)
        {
            this.repositorio = repositorio ?? throw new ArgumentNullException(nameof(repositorio));
        }
        public Task<IEnumerable<ArquivoDto>> Handle(ObterAnexosEncaminhamentoNAAPAQuery request, CancellationToken cancellationToken)
        {
            return repositorio.ObterRepostasArquivosPdfPorEncaminhamentoIdAsync(request.EncaminhamentoId, request.ImprimirAnexosNAAPA);
        }
    }
}
