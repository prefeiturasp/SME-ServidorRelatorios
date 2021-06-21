using MediatR;
using SME.SR.Data;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterArquivoPorCodigoQueryHandler : IRequestHandler<ObterArquivoPorCodigoQuery, ArquivoDto>
    {
        private readonly IArquivoRepository arquivoRepository;

        public ObterArquivoPorCodigoQueryHandler(IArquivoRepository arquivoRepository)
        {
            this.arquivoRepository = arquivoRepository ?? throw new ArgumentNullException(nameof(arquivoRepository));
        }

        public async Task<ArquivoDto> Handle(ObterArquivoPorCodigoQuery request, CancellationToken cancellationToken)
        {
            var arquivo = await arquivoRepository.ObterPorCodigo(request.Codigo);

            return arquivo;
        }
    }
}
