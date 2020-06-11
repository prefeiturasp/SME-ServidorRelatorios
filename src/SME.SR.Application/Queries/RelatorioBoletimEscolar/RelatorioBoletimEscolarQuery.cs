using MediatR;
using SME.SR.Infra.Dtos.Relatorios.BoletimEscolar;
using SME.SR.Infra.Dtos.Relatorios.ConselhoClasse;

namespace SME.SR.Application.Queries.RelatorioBoletimEscolar
{
    public class RelatorioBoletimEscolarQuery : IRequest<BoletimEscolarDto>
    {
    }
}
