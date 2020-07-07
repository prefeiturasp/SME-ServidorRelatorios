using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Data.Models;
using SME.SR.Infra;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterParametrosMediaFrequenciaQueryHandler : IRequestHandler<ObterParametrosMediaFrequenciaQuery, IEnumerable<MediaFrequencia>>
    {
        private readonly IParametroSistemaRepository parametroSistemaRepository;

        public ObterParametrosMediaFrequenciaQueryHandler(IParametroSistemaRepository parametroSistemaRepository)
        {
            this.parametroSistemaRepository = parametroSistemaRepository ?? throw new ArgumentNullException(nameof(parametroSistemaRepository));
        }

        public async Task<IEnumerable<MediaFrequencia>> Handle(ObterParametrosMediaFrequenciaQuery request, CancellationToken cancellationToken)
        {
            var mediasFrequencia =  await parametroSistemaRepository.ObterMediasFrequencia();

            if (mediasFrequencia == null || !mediasFrequencia.Any())
                throw new NegocioException("Não foi possível encontrar as médias de frequência");

            return mediasFrequencia;
        }
    }
}
