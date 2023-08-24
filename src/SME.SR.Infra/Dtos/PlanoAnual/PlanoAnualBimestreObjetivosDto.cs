using System;
using System.Collections.Generic;

namespace SME.SR.Infra
{
    public class PlanoAnualBimestreObjetivosDto
    {
        public long Id { get; set; }
        public string UeNome { get; set; }
        public string UeCodigo { get; set; }
        public TipoEscola TipoEscola { get; set; }
        public string DreNome { get; set; }
        public Modalidade ModalidadeTurma { get; set; }
        public string TurmaNome { get; set; }
        public int TurmaTipoTurno { get; set; }
        public string ComponenteCurricular { get; set; }
        public string Usuario { get; set; }
        public int AnoLetivo { get; set; }
        public int Bimestre { get; set; }
        public string DescricaoPlanejamento { get; set; }
        public string ObjetivoCodigo { get; set; }
        public string ObjetivoDescricao { get; set; }
        public string DataImpressao { get; set; } = DateTimeExtension.HorarioBrasilia().Date.ToString("dd/MM/yyyy");
    }
}
