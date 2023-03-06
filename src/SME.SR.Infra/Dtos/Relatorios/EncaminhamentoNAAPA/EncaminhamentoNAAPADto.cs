using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Linq;

namespace SME.SR.Infra
{
    public class EncaminhamentoNAAPADto
    {
        public long Id { get; set; }    
        public long DreId { get; set; }
        public string DreAbreviacao { get; set; }
        public string UeCodigo { get; set; }
        public string UeNome { get; set; }
        public TipoEscola TipoEscola { get; set; }
        public string AlunoCodigo { get; set; }
        public string AlunoNome { get; set; }
        public int Situacao { get; set; }
        public string TurmaCodigo { get; set; }
        public string TurmaNome { get; set; }
        public Modalidade Modalidade { get; set; }
        public DateTime DataEntradaQueixa { get; set; }
        public string PortaEntrada { get; set; }
        public string[] FluxosAlerta { get; set; }
        public DateTime DataUltimoAtendimento { get; set; }

        public void AdicionarFluxoAlerta(string fluxoAlerta)
        {
            if (!FluxosAlerta.Contains(fluxoAlerta))
                FluxosAlerta.Append(fluxoAlerta);
        }
    }
}
