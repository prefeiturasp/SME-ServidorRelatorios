﻿using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterEnderecoEAtosDaUeQueryHandler : IRequestHandler<ObterEnderecoEAtosDaUeQuery, IEnumerable<EnderecoEAtosDaUeDto>>
    {
        private readonly IObterEnderecoeAtosDaUeRepository ObterEnderecoeAtosDaUeRepository;

        public ObterEnderecoEAtosDaUeQueryHandler(IObterEnderecoeAtosDaUeRepository obterEnderecoeAtosDaUeRepository)
        {
            ObterEnderecoeAtosDaUeRepository = obterEnderecoeAtosDaUeRepository;
        }

        public async Task<IEnumerable<EnderecoEAtosDaUeDto>> Handle(ObterEnderecoEAtosDaUeQuery request, CancellationToken cancellationToken)
        {
            return await ObterEnderecoeAtosDaUeRepository.ObterEnderecoEAtos(request.UeCodigo);
        }
    }
}
