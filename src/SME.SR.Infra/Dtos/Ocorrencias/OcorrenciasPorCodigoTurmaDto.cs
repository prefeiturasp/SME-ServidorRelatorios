using System;

namespace SME.SR.Infra
{
    public class OcorrenciasPorCodigoTurmaDto
    {
        public long OcorrenciaId { get; set; }
        public long TurmaId { get; set; }
        public string OcorrenciaTitulo { get; set; }
        public DateTime OcorrenciaData { get; set; }
        public string OcorrenciaDescricao { get; set; }
        public string OcorrenciaTipo { get; set; }
        public long CodigoAluno { get; set; }
        public string OcorrenciaDataFormatada() => OcorrenciaData.ToString("dd/MM/yyyy");
    }
}
