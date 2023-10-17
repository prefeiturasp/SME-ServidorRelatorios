using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra.Dtos.FrequenciaMensal;

namespace SME.SR.Application
{
    public class ObterFrequenciaRealatorioControleMensalQueryHandler : IRequestHandler<ObterFrequenciaRealatorioControleMensalQuery, IEnumerable<ConsultaRelatorioFrequenciaControleMensalDto>>
    {
        private readonly IFrequenciaAlunoRepository frequenciaRepository;
        private readonly IMediator mediator;

        public ObterFrequenciaRealatorioControleMensalQueryHandler(IFrequenciaAlunoRepository frequenciaRepository, IMediator mediator)
        {
            this.frequenciaRepository = frequenciaRepository ?? throw new ArgumentNullException(nameof(frequenciaRepository));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IEnumerable<ConsultaRelatorioFrequenciaControleMensalDto>> Handle(ObterFrequenciaRealatorioControleMensalQuery request, CancellationToken cancellationToken)
        {
            var retorno = await frequenciaRepository.ObterFrequenciaControleMensal(request.AnoLetivo, request.Mes, request.UeCodigo, request.DreCodigo, request.Modalidade,
                request.Semestre, request.TurmaCodigo, request.AlunosCodigo);

            return await TratarInformacoesComponentesCurriculares(retorno.ToList());
        }

        private async Task<IEnumerable<ConsultaRelatorioFrequenciaControleMensalDto>> TratarInformacoesComponentesCurriculares(List<ConsultaRelatorioFrequenciaControleMensalDto> frequenciasControleMensal)
        {

            var idsComponentesSemNome = frequenciasControleMensal.Where(ff => string.IsNullOrEmpty(ff.NomeComponente)).Select(ff => long.Parse(ff.DisciplinaId)).Distinct();
            if (!idsComponentesSemNome.Any())
                return frequenciasControleMensal;

            var informacoesComponentesCurriculares = await mediator.Send(new ObterComponentesCurricularesEolPorIdsQuery(idsComponentesSemNome.ToArray()));
            foreach (var frequenciaGlobal in frequenciasControleMensal.Where(ff => string.IsNullOrEmpty(ff.NomeComponente)))
            {
                var componenteCurricular = informacoesComponentesCurriculares.Where(cc => cc.CodDisciplina.ToString() == frequenciaGlobal.DisciplinaId).FirstOrDefault();
                if (!(componenteCurricular is null))
                {
                    frequenciaGlobal.NomeComponente = componenteCurricular.Disciplina;
                    frequenciaGlobal.NomeGrupo = componenteCurricular.GrupoMatriz?.Nome;
                }
            }
            return frequenciasControleMensal;
        }
    }
}