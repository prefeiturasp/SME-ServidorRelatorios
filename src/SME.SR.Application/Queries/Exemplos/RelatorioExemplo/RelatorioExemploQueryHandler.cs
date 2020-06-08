using MediatR;
using SME.SR.Infra.Dtos.Relatorios.ConselhoClasse;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application.Queries.Exemplos.RelatorioExemplo
{
    public class RelatorioExemploQueryHandler : IRequestHandler<RelatorioExemploQuery, ConselhoClasseDto>
    {
        public Task<ConselhoClasseDto> Handle(RelatorioExemploQuery request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new ConselhoClasseDto()
            {
                Cabecalho = new ConselhoClasseCabecalhoDto(
                    "Nome da dre",
                    "Nome da ue",
                    new DateTime(2020, 05, 01)
                ),
            });
        }
    }
}
