using MediatR;
using SME.SR.Data.Interfaces;
using SME.SR.Infra;
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
            var devolutivas = await devolutivaRepository.ObterDevolutivas(request.UeId, request.Turmas, request.Bimestres);

            foreach (var devolutivasPorTurma in devolutivas.GroupBy(a => a.Aula.Turma))
            {
                devolutivasDto.Add(new TurmasDevolutivasDto()
                {
                    NomeTurma = devolutivasPorTurma.Key.Nome,
                    Bimestres = ObterBimestres(devolutivasPorTurma)
                });
            }

            return devolutivasDto;
        }

        private IEnumerable<BimestresDevolutivasDto> ObterBimestres(IGrouping<TurmaNomeDto, DevolutivaDto> devolutivas)
        {
            foreach (var devolutivasPorBimestre in devolutivas.GroupBy(a => a.Aula.PeriodoEscolar))
            {
                var periodoEscolar = devolutivasPorBimestre.Key;

                yield return new BimestresDevolutivasDto()
                {
                    NomeBimestre = $"{periodoEscolar.Bimestre}º BIMESTRE ({periodoEscolar.DataInicio:dd/MM/yyyy} À {periodoEscolar.Bimestre:dd/MM/yyyy})",
                    Devolutivas = ObterDevolutivasQuery(devolutivasPorBimestre)
                };
            }
        }

        private IEnumerable<DevolutivaRelatorioDto> ObterDevolutivasQuery(IGrouping<PeriodoEscolarDto, DevolutivaDto> devolutivas)
        {
            foreach(var devolutivasPorId in devolutivas.GroupBy(a => a.Id))
            {
                var datas = string.Join(", ", devolutivasPorId.SelectMany(a => a.Aula.Data.ToString("dd/MM/yyyy")));
                var devolutiva = devolutivasPorId.First();

                yield return new DevolutivaRelatorioDto()
                {
                    IntervaloDatas = $"{devolutiva.DataInicio:dd/MM/yyyy} até {devolutiva.DataFim:dd/MM/yyyy}",
                    DiasIntervalo = datas,
                    DataRegistro = devolutiva.DataRegistro.ToString("dd/MM/yyyy"),
                    ResgistradoPor = devolutiva.RegistradoPor,
                    Descricao = devolutiva.Descricao
                };
            }
        }
    }
}
