using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterRecuperacaoParalelaPeriodoPorIdQueryHandler : IRequestHandler<ObterRecuperacaoParalelaPeriodoPorIdQuery, RecuperacaoParalelaPeriodoDto>
    {
        private readonly IRecuperacaoParalelaRepository recuperacaoParalelaRepository;

        public ObterRecuperacaoParalelaPeriodoPorIdQueryHandler(IRecuperacaoParalelaRepository recuperacaoParalelaRepository)
        {
            this.recuperacaoParalelaRepository = recuperacaoParalelaRepository ?? throw new ArgumentNullException(nameof(recuperacaoParalelaRepository));
        }

        public async Task<RecuperacaoParalelaPeriodoDto> Handle(ObterRecuperacaoParalelaPeriodoPorIdQuery request, CancellationToken cancellationToken)
        {
            var recuperacaoParalelaPeriodo = await recuperacaoParalelaRepository.ObterPeriodoPorId(request.RecuperacaoParalelaPeriodoId);

            if (recuperacaoParalelaPeriodo == null)
                throw new NegocioException("Não foi possível obter o período da recuperação paralela");

            return MapearParaDto(recuperacaoParalelaPeriodo);
        }

        private RecuperacaoParalelaPeriodoDto MapearParaDto(RecuperacaoParalelaPeriodo recuperacaoParalelaPeriodo)
        {
            return new RecuperacaoParalelaPeriodoDto()
            {
                BimestreEdicao = recuperacaoParalelaPeriodo.BimestreEdicao,
                Descricao = recuperacaoParalelaPeriodo.Descricao,
                Id = recuperacaoParalelaPeriodo.Id,
                Nome = recuperacaoParalelaPeriodo.Nome
            };
        }
    }
}