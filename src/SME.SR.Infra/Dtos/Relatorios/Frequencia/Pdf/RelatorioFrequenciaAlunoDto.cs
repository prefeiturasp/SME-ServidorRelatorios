using System;
using System.Globalization;

namespace SME.SR.Infra
{
    public class RelatorioFrequenciaAlunoDto
    {
        public const int PERCENTUAL_FREQUENCIA_PRECISAO = 2;
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

                if (TotalAulas == TotalAusencias && TotalCompensacoes == 0)
                    return "0";

                if (TotalPresenca == 0 && TotalRemoto == 0 && TotalAusencias == 0)
                    return "";

                var porcentagem = 100 - Math.Round((double)NumeroFaltasNaoCompensadas / TotalAulas, 2) * 100;
                var porcentagemRetorno = Math.Round(porcentagem > 100 ? 100 : porcentagem, 2);

                return porcentagemRetorno.ToString($"N{PERCENTUAL_FREQUENCIA_PRECISAO}", CultureInfo.CurrentCulture);
            }
        }        
    }
}
