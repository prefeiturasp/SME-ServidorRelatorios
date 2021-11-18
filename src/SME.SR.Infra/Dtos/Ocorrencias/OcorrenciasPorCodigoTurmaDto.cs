using System;

namespace SME.SR.Infra
{
    public class OcorrenciasPorCodigoTurmaDto
    {
        public int OcorenciaCodigo { get; set; }
        public int TurmaCodigo { get; set; }
        public string OcorrenciaTitulo { get; set; }
        public DateTime OcorrenciaData { get; set; }
        public string OcorrenciaDescricao { get; set; }
        public string OcorrenciaTipo { get; set; }
        public int CodigoAluno { get; set; }
        public string OcorrenciaDataFormatada() => OcorrenciaData.ToString("dd/MM/yyyy");
    }
}
