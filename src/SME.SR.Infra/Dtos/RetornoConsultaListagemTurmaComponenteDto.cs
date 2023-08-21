using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra.Dtos
{
    public class RetornoConsultaListagemTurmaComponenteDto
    {
        public string Id { get; set; }
        public string TurmaCodigo { get; set; }
        public int Modalidade { get; set; }
        public string NomeTurma { get; set; }
        public string Ano { get; set; }
        public string NomeComponenteCurricular { get; set; }
        public long ComponenteCurricularCodigo { get; set; }
        public string ComplementoTurmaEJA { get; set; }
        public long ComponenteCurricularTerritorioSaberCodigo { get; set; }
        public string Turno { get; set; }
        public bool TerritorioSaber { get; set; }
        public int TotalRegistros { get; set; }
        public string RegistroFuncional { get; set; }
        public bool Historica { get; set; }
        public int TipoEscola { get; set; }
        public string SituacaoTurmaEscola { get; set; }
        public DateTime DataStatusTurmaEscola { get; set; }
        public string CodigoEscola { get; set; }
        public int AnoLetivo { get; set; }
        public DateTime? DataDisponibizacao { get; set; }
        public int EtapaEnsino { get; set; }

        public int TipoGradePrograma { get; set; }

        public int CodigoGradePrograma { get; set; }

        public string DescricaoGradePrograma { get; set; }

        public string SerieEnsino { get; set; }

        public string NomeFiltro { get; set; }

        public DateTime? DataInicioTurma { get; set; }
        public DateTime? DataFimTurma { get; set; }
        public int CicloEnsino { get; set; }

        public int  TipoTurma { get; set; }

        public int DuracaoTurno { get; set; }
        
        public DateTime DataAtualizacao { get; set; }
        public int EnsinoEspecial { get; set; }
        public int Semestre { get; set; }
        public int Extinta { get; set; }
        
        public int EtapaEJA
        {
            get
            {
                if (Modalidade == (int)SME.SR.Infra.Modalidade.EJA && !string.IsNullOrEmpty(SerieEnsino) && SerieEnsino.Length > 2)
                {
                    var etapa = SerieEnsino.Substring(SerieEnsino.Length - 2).Trim();

                    var indexPrimeiroCiclo = SerieEnsino.IndexOf(" I ");
                    var indexSegundoCiclo = SerieEnsino.IndexOf(" II");

                    if ((etapa == "I" && indexSegundoCiclo < 0) || (indexPrimeiroCiclo >= 0 && indexSegundoCiclo < 0))
                        return 1;
                    else if ((etapa == "II" && indexPrimeiroCiclo < 0) || (indexPrimeiroCiclo < 0 && indexSegundoCiclo >= 0))
                        return 2;
                }
                return 0;

            }
        }
    }
}
