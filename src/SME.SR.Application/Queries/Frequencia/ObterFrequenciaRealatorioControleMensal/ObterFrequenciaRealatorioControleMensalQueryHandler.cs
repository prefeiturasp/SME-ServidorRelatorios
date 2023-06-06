using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra.Dtos.FrequenciaMensal;

namespace SME.SR.Application
{
    public class ObterFrequenciaRealatorioControleMensalQueryHandler : IRequestHandler<ObterFrequenciaRealatorioControleMensalQuery,IEnumerable<ConsultaRelatorioFrequenciaControleMensalDto>>
    {
        private readonly IFrequenciaAlunoRepository frequenciaRepository;

        public ObterFrequenciaRealatorioControleMensalQueryHandler(IFrequenciaAlunoRepository frequenciaRepository)
        {
            this.frequenciaRepository = frequenciaRepository ?? throw new ArgumentNullException(nameof(frequenciaRepository));
        }

        public async Task<IEnumerable<ConsultaRelatorioFrequenciaControleMensalDto>> Handle(ObterFrequenciaRealatorioControleMensalQuery request, CancellationToken cancellationToken)
        {
            return await frequenciaRepository.ObterFrequenciaControleMensal(request.AnoLetivo,  request.Mes,  request.UeCodigo,  request.DreCodigo,  request.Modalidade, 
                request.Semestre, request.TurmaCodigo, request.AlunosCodigo);
        }
    }
}