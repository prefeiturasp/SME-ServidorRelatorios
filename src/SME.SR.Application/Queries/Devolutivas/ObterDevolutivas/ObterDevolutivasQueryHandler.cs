using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDevolutivasQueryHandler : IRequestHandler<ObterDevolutivasQuery, IEnumerable<TurmasDevolutivasDto>>
    {
        private readonly IDevolutivaRepository devolutivaRepository;

        public ObterDevolutivasQueryHandler(IDevolutivaRepository devolutivaRepository)
        {
            this.devolutivaRepository = devolutivaRepository ?? throw new ArgumentNullException(nameof(devolutivaRepository));
        }

        public async Task<IEnumerable<TurmasDevolutivasDto>> Handle(ObterDevolutivasQuery request, CancellationToken cancellationToken)
        {
            var devolutivasDto = new List<TurmasDevolutivasDto>();
            var devolutivas = await devolutivaRepository.ObterDevolutivas(request.UeId, request.Turmas, request.Bimestres, request.Ano, request.ComponenteCurricular, request.UtilizarLayoutNovo);

            if (devolutivas == null || !devolutivas.Any())
                throw new NegocioException("Nenhuma informação para os filtros informados.");

            foreach (var devolutivasPorTurma in devolutivas.GroupBy(a => new { a.Aula.Turma.Id, a.Aula.Turma.Nome }))
            {
                devolutivasDto.Add(new TurmasDevolutivasDto()
                {
                    NomeTurma = devolutivasPorTurma.Key.Nome,
                    BimestresComponentesCurriculares = ObterBimestresComponentesCurriculares(devolutivasPorTurma, request.UtilizarLayoutNovo),
                });
            }

            return devolutivasDto;
        }

        private IEnumerable<BimestresComponentesCurricularesDevolutivasDto> ObterBimestresComponentesCurriculares(IGrouping<object, DevolutivaDto> devolutivas, bool utilizarLayoutNovo)
        {
            foreach (var devolutivasPorBimestre in devolutivas
                .GroupBy(a => new { BimestreComponenteCurricular = utilizarLayoutNovo ? a.ComponenteCurricular : a.Aula.PeriodoEscolar.Bimestre.ToString(), a.Aula.PeriodoEscolar.DataInicio, a.Aula.PeriodoEscolar.DataFim })
                .OrderBy(a => a.Key.BimestreComponenteCurricular))
            {
                var bimestreComponenteCurricular = devolutivasPorBimestre.Key;

                yield return new BimestresComponentesCurricularesDevolutivasDto()
                {
                    NomeBimestreComponenteCurricular = $"{bimestreComponenteCurricular.BimestreComponenteCurricular}{IncluirComplemento(utilizarLayoutNovo)} ({bimestreComponenteCurricular.DataInicio:dd/MM/yyyy} À {bimestreComponenteCurricular.DataFim:dd/MM/yyyy})",
                    Devolutivas = ObterDevolutivasQuery(devolutivasPorBimestre).ToList()
                };
            }
        }

        private string IncluirComplemento(bool utilizarLayoutNovo)
        {
            return utilizarLayoutNovo ? string.Empty : "º BIMESTRE";
        }

        private IEnumerable<DevolutivaRelatorioDto> ObterDevolutivasQuery(IGrouping<object, DevolutivaDto> devolutivas)
        {
            foreach(var devolutivasPorId in devolutivas
                .GroupBy(a => new { a.Id, a.DataInicio })
                .OrderByDescending(a => a.Key.DataInicio))
            {
                var datas = string.Join(", ", devolutivasPorId
                    .OrderBy(a => a.Aula.Data)
                    .Select(a => a.Aula.Data.ToString("dd/MM")));
                var devolutiva = devolutivasPorId.First();

                yield return new DevolutivaRelatorioDto()
                {
                    IntervaloDatas = $"{devolutiva.DataInicio:dd/MM/yyyy} até {devolutiva.DataFim:dd/MM/yyyy}",
                    DiasIntervalo = datas,
                    DataRegistro = devolutiva.DataRegistro.ToString("dd/MM/yyyy"),
                    ResgistradoPor = $"{devolutiva.RegistradoPor} ({devolutiva.RegistradoRF})",
                    Descricao = UtilRegex.AdicionarEspacos(
                        UtilRegex.RemoverTagsHtml(
                        UtilRegex.RemoverTagsHtmlMidia(devolutiva.Descricao)))
                };
            }
        }
    }
}
