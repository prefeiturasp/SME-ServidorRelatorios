using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SME.SR.Infra;

namespace SME.SR.Data
{
    public interface ICalendarioEventoRepository
    {
        Task<IEnumerable<CalendarioEventoQueryRetorno>> ObterEventosPorUsuarioTipoCalendarioPerfilDreUe(string usuarioLogin, Guid usuarioPerfil, bool consideraHistorico, bool consideraPendenteAprovacao, string dreCodigo, string ueCodigo, bool desconsideraEventoSme, bool desconsideraLocalDre, long tipoCalendarioId);
    }
}