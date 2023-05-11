using System;
using System.Threading.Tasks;
using MediatR;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;

namespace SME.SR.Application.UseCases
{
    public class RelatorioFrequenciaControleMensalUseCase : IRelatorioFrequenciaControleMensalUseCase
    {
        private readonly IMediator mediator;

        public RelatorioFrequenciaControleMensalUseCase(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task Executar(FiltroRelatorioDto request)
        {
            /*
             * Cabecalho
             * Nome Dre
             * Nome Ue
             * Criança/Estudante
             * CodigoEOL
             * Turma
             * Mês
             */
        }
    }
}