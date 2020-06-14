using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace SME.SR.Infra.Dtos.Relatorios.BoletimEscolar
{
    public class ComponenteCurricularDto
    {
        public string Nome { get; set; }

        private string TipoNota { get; set; } = "Sem Nota";

        public void SetarTipoNota(string notaConceito)
        {
            if (!string.IsNullOrEmpty(notaConceito))
            {
                if (double.TryParse(notaConceito, out _))
                    TipoNota = "Nota";
                else
                    TipoNota = "Conceito";
            }
        }

        public string NotaBimestre1 { get; set; }

        public string FrequenciaBimestre1 { get; set; }

        public string NotaBimestre2 { get; set; }

        public string FrequenciaBimestre2 { get; set; }

        public string NotaBimestre3 { get; set; }

        public string FrequenciaBimestre3 { get; set; }

        public string NotaBimestre4 { get; set; }

        public string FrequenciaBimestre4 { get; set; }

        public string NotaFinal { get; set; }

        public string FrequenciaFinal { get; set; }
    }
}
