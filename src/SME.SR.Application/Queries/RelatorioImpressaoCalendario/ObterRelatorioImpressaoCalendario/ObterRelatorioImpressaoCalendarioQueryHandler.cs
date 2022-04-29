using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Globalization;

namespace SME.SR.Application
{
    public class ObterRelatorioImpressaoCalendarioQueryHandler : IRequestHandler<ObterRelatorioImpressaoCalendarioQuery, RelatorioImpressaoCalendarioDto>
    {
        private readonly ICalendarioEventoRepository calendarioEventoRepository;

        public ObterRelatorioImpressaoCalendarioQueryHandler(ICalendarioEventoRepository calendarioEventoRepository)
        {
            this.calendarioEventoRepository = calendarioEventoRepository ?? throw new ArgumentNullException(nameof(calendarioEventoRepository));
        }
        public async Task<RelatorioImpressaoCalendarioDto> Handle(ObterRelatorioImpressaoCalendarioQuery request, CancellationToken cancellationToken)
        {
            var consideraHistorico = request.TipoCalendario.AnoLetivo != DateTime.Today.Year;


            string dreCodigo = "", ueCodigo = "";
            if (request.Dre != null)
                dreCodigo = request.Dre.Codigo;

            if (request.Ue != null)
                ueCodigo = request.Ue.Codigo;


            var retornoQuery = await calendarioEventoRepository.ObterEventosPorUsuarioTipoCalendarioPerfilDreUe(request.UsuarioRF, request.UsuarioPerfil, consideraHistorico, request.ConsideraPendenteAprovacao,
                dreCodigo, ueCodigo, !request.EhSME, request.PodeVisualizarEventosOcorrenciaDre, request.TipoCalendario.Id);

            var retorno = MontarCabecalho(request.Dre, request.Ue, request.TipoCalendario);

            var meses = retornoQuery.Select(a => a.DataInicio.Month).Distinct().OrderBy( a => a);

            CultureInfo cultureinfo = new CultureInfo("pt-BR");
            DateTimeFormatInfo dtfi = cultureinfo.DateTimeFormat;            

            foreach (var mes in meses)
            {
                var mesParaIncluir = new RelatorioImpressaoCalendarioMesDto();
                mesParaIncluir.MesDescricao = new DateTime(2020, mes, 1).ToString("MMMM", cultureinfo).ToUpper();
                mesParaIncluir.MesNumero = mes;

                var eventosDoMes = retornoQuery.Where(a => a.DataInicio.Month == mes);
                foreach (var eventoDoMes in eventosDoMes.OrderBy( a => a.DataInicio))
                {
                    var eventoParaIncluir = new RelatorioImpressaoCalendarioEventoDto();
                    eventoParaIncluir.Dia = eventoDoMes.DataInicio.Day.ToString().PadLeft(2,'0');
                    eventoParaIncluir.DiaSemana = dtfi.GetAbbreviatedDayName(eventoDoMes.DataInicio.DayOfWeek).ToUpper();
                    eventoParaIncluir.Evento = eventoDoMes.Nome;
                    eventoParaIncluir.EventoTipo = eventoDoMes.TipoEvento;                        
                    mesParaIncluir.Eventos.Add(eventoParaIncluir);
                }

                retorno.Meses.Add(mesParaIncluir);
            }

            return retorno;
        }

        private RelatorioImpressaoCalendarioDto MontarCabecalho(Dre dre, Ue ue, TipoCalendarioDto tipoCalendarioDto)
        {
            var retorno = new RelatorioImpressaoCalendarioDto();

            retorno.DreNome = dre == null ? "TODAS" : dre.Abreviacao;
            retorno.TipoCalendarioNome = tipoCalendarioDto.Nome;
            retorno.UeNome = ue == null ? "TODAS" : ue.NomeComTipoEscola;

            return retorno;
        }
    }
}
