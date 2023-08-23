using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class RelatorioPlanoAnualDto
    {
        public string UeNome { get; set; }
        public string DreNome { get; set; }
        public string ComponenteCurricular { get; set; }
        public string Usuario { get; set; }
        public int AnoLetivo { get; set; }
        public string DataImpressao { get; set; } = DateTimeExtension.HorarioBrasilia().Date.ToString("dd/MM/yyyy");
        public string Turma { get; set; }
        public IEnumerable<BimestreDescricaoPlanejamentoDto> Bimestres { get; set; }
        public bool ExibeObjetivos { get; set; }
    }
}
