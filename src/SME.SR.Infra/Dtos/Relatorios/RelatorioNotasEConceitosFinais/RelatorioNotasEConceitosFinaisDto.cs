using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioNotasEConceitosFinaisDto
    {
        public RelatorioNotasEConceitosFinaisDto()
        {
            Data = DateTime.Now.ToString("dd/MM/yyyy");
            Dres = new List<RelatorioNotasEConceitosFinaisDreDto>();
        }

        public string DreNome { get; set; }
        public string UeNome { get; set; }
        public string Ano { get; set; }
        public string Bimestre { get; set; }
        public string ComponenteCurricular { get; set; }
        public bool PossuiNotaFechamento { get; set; }
        public string UsuarioNome { get; set; }
        public string UsuarioRF { get; set; }
        public string Data { get; set; }

        public List<RelatorioNotasEConceitosFinaisDreDto> Dres { get; set; }
    }
}
