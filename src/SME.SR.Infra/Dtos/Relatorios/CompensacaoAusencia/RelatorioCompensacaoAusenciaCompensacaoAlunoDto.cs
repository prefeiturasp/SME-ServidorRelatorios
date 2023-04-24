using System;

namespace SME.SR.Infra
{
    public class RelatorioCompensacaoAusenciaCompensacaoAlunoDto
    {
        public string NumeroChamada { get; set; }
        public string NomeAluno { get; set; }
        public int TotalAusencias { get; set; }
        public int TotalCompensacoes { get; set; }
        public int TotalAulas { get; set; }
        public int NumeroFaltasNaoCompensadas { get => TotalAusencias - TotalCompensacoes; }
        public double Frequencia
        {
            get
            {
                if (TotalAulas == 0)
                    return 0;

                var porcentagem = 100 - ((double)NumeroFaltasNaoCompensadas / TotalAulas) * 100;

                return Math.Round(porcentagem, 2);
            }
        }
        public string FrequenciaFormatado
        {
            get
            {
                return Frequencia.ToString("N2");
            }
        }
    }
}
