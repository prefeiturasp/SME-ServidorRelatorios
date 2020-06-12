using MediatR;
using SME.SR.Infra.Dtos.Relatorios.BoletimEscolar;
using SME.SR.Infra.Dtos.Relatorios.ConselhoClasse;

namespace SME.SR.Application.Queries.BoletimEscolar
{
    public class ObterRelatorioBoletimEscolarQuery : IRequest<BoletimEscolarDto>
    {
    }
}
