using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra.Dtos.SondagemTurmaEscritaEF
{
    public class EscritaEfTurmaSondagemCabecalhoExcelDto
    {
        public EscritaEfTurmaSondagemCabecalhoExcelDto()
        {
            CorpoRelatorio = new List<EscritaEfTurmaSondagemCorpoExcelDto>();
        }
        public int AnoLetivo { get; set; }
        public string Semestre { get; set; }
        public string Turma { get; set; }
        public string Ue { get; set; }
        public string Dre { get; set; }
        public string Modalidade { get; set; }
        public string Proeficiencia { get; set; }
        public string DataImpressao { get; set; }
        public string NomeUsuarioSolicitacao { get; set; }
        public List<EscritaEfTurmaSondagemCorpoExcelDto> CorpoRelatorio { get; set; }
    }

    public class EscritaEfTurmaSondagemCorpoExcelDto
    {
        public string Numero { get; set; }
        public string Nome { get; set; }
        public string Raca { get; set; }
        public string Genero { get; set; }
        public string LpComoLinguaPrincipal { get; set; }
        public string SondagemInicial { get; set; }
        public string PrimeiroBimestre { get; set; }
        public string SegundoBimestre { get; set; }
        public string TerceiroBimestre { get; set; }
        public string QuartoBimestre { get; set; }
        public string Cor { get; set; }
    }

    public class GraficoDto
    {
        public string Descricao { get; set; } = string.Empty;
        public string Cor { get; set; } = string.Empty;
        public int Quantidade { get; set; }
    }
}
