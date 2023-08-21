using System.Collections.Generic;

namespace SME.SR.Infra.Dtos
{
    public struct PaginacaoResultadoDto<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int TotalPaginas { get; set; }
        public int TotalRegistros { get; set; }
    }
}
