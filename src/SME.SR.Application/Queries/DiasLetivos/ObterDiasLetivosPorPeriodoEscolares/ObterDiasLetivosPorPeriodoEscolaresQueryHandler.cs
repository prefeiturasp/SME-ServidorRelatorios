using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class ObterDiasLetivosPorPeriodoEscolaresQueryHandler : IRequestHandler<ObterDiasLetivosPorPeriodoEscolaresQuery, List<DiaLetivoDto>>
    {
        private const int REPOSICAO_AULA = 13;
        private readonly IEventoRepository repositorioEvento;

        public ObterDiasLetivosPorPeriodoEscolaresQueryHandler(IEventoRepository repositorioEvento)
        {
            this.repositorioEvento = repositorioEvento ?? throw new ArgumentNullException(nameof(repositorioEvento));
        }

        public async Task<List<DiaLetivoDto>> Handle(ObterDiasLetivosPorPeriodoEscolaresQuery request, CancellationToken cancellationToken)
        {
            var datasDosPeriodosEscolares = new List<DiaLetivoDto>();

            var tiposLetivosConsiderados = new int[] { (int)EventoLetivo.Sim, (int)EventoLetivo.Nao };

            var eventosTipoCalendario = await repositorioEvento.ObterEventosPorTipoDeCalendarioAsync(request.TipoCalendarioId, request.UeCodigo, tiposLetivosConsiderados.Select(tl => (EventoLetivo)tl).ToArray());

            var eventos = eventosTipoCalendario?
                .Where(c => c.EhEventoUE() || c.EhEventoSME());

            DefinirDiasLetivos(request.PeriodosEscolares, request.DesconsiderarCriacaoDiaLetivoProximasUes, datasDosPeriodosEscolares, eventos);

            return datasDosPeriodosEscolares;
        }

        private static void DefinirDiasLetivos(IEnumerable<PeriodoEscolar> periodosEscolares, bool desconsiderarCriacaoDiaLetivoProximasUes, List<DiaLetivoDto> datasDosPeriodosEscolares, IEnumerable<Evento> eventos)
        {
            foreach (var periodoEscolar in periodosEscolares.OrderBy(c => c.Bimestre))
            {
                foreach (var diaAtual in periodoEscolar.ObterIntervaloDatas())
                {
                    var diaLetivoDto = new DiaLetivoDto()
                    {
                        Data = diaAtual,
                        PossuiEvento = false
                    };

                    if (diaAtual.Date.Equals(new DateTime(2023, 6, 13)))
                    {
                        _ = "debug";
                    }
                    var eventosComData = eventos.Where(e => diaAtual.Date >= e.DataInicio.Date
                                                            && diaAtual.Date <= e.DataFim.Date)
                                                .ToList();
                    PreencherInformacoesDiaLetivo(diaLetivoDto, eventosComData, datasDosPeriodosEscolares, desconsiderarCriacaoDiaLetivoProximasUes, diaAtual);
                }
            }
        }

        private static void PreencherInformacoesDiaLetivo(DiaLetivoDto diaLetivoDto, List<Evento> eventosComData,
                                             List<DiaLetivoDto> datasDosPeriodosEscolares,
                                             bool desconsiderarCriacaoDiaLetivoProximasUes,
                                             DateTime diaPeriodo)
        {
            if (!eventosComData.Any())
            {
                diaLetivoDto.EhLetivo = !diaPeriodo.FimDeSemana();
                datasDosPeriodosEscolares.Add(diaLetivoDto);
                return;
            }

            var eventosSME = eventosComData.Where(e => e.EhEventoSME());
            if (eventosSME.Any())
            {
                diaLetivoDto.EhLetivo = eventosSME.Any(e => e.EhEventoLetivo());
                diaLetivoDto.Motivo = eventosSME.First().Nome;
                diaLetivoDto.PossuiEvento = true;
                datasDosPeriodosEscolares.Add(diaLetivoDto);
                return;
            }

            var eventosDRE = eventosComData.Where(e => e.EhEventoDRE());
            if (eventosDRE.Any())
            {
                diaLetivoDto.EhLetivo = eventosDRE.Any(e => e.EhEventoLetivo());
                diaLetivoDto.Motivo = eventosDRE.First().Nome;
                diaLetivoDto.PossuiEvento = true;
                datasDosPeriodosEscolares.Add(diaLetivoDto);
                return;
            }

            var eventosLetivosNaoLetivosUE = eventosComData.Where(e => e.EhEventoUE());

            if (eventosLetivosNaoLetivosUE.Any())
            {
                eventosLetivosNaoLetivosUE.ToList().ForEach(elue =>
                {
                    datasDosPeriodosEscolares.Add(new DiaLetivoDto()
                    {
                        Data = diaPeriodo,
                        PossuiEvento = true,
                        EhLetivo = EventoEhLetivo(diaPeriodo, elue),
                        Motivo = elue.Nome,
                        UesIds = new List<string>() { elue.UeId }
                    });
                });

                if (desconsiderarCriacaoDiaLetivoProximasUes)
                    return;
            }
            else
            {
                diaLetivoDto.EhLetivo = !diaPeriodo.FimDeSemana();
                datasDosPeriodosEscolares.Add(diaLetivoDto);
            }
        }

        private static bool EventoEhLetivo(DateTime data, Evento evento)
            => (!data.FimDeSemana() && evento.EhEventoLetivo()) ||
               (data.FimDeSemana() && (evento.EhEventoLetivo() || evento.TipoEventoId == REPOSICAO_AULA));
    }
}
