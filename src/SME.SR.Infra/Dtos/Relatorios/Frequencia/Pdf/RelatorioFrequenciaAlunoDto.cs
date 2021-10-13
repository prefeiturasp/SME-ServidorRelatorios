using System;

namespace SME.SR.Infra
{
    public class RelatorioFrequenciaAlunoDto
    {
        public string NumeroChamada { get; set; }
        public int CodigoAluno { get; set; }
        public string NomeAluno { get; set; }
        public string NomeTurma { get; set; }
        public string CodigoTurma { get; set; }
        public int TotalAusencias { get; set; }
        public int TotalRemoto { get; set; }
        public int TotalCompensacoes { get; set; }
        public int TotalAulas { get; set; }
        public int TotalPresenca { get; set; }
        public int NumeroFaltasNaoCompensadas { get => TotalAusencias - TotalCompensacoes; }
        public string Ano { get; set; }
        public string FrequenciaFormatada { get; set; }
        public string Frequencia
        {
            get
            {
                if (TotalAulas == 0)
                    return "";

                var porcentagem = 100 - ((double)NumeroFaltasNaoCompensadas / TotalAulas) * 100;

                return Math.Round(porcentagem > 100 ? 100 : porcentagem, 2).ToString();
            }
        }        
    }
}
