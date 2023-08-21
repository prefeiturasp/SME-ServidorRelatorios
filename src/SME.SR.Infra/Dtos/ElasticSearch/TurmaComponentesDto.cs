using Nest;
using System;
using System.Collections.Generic;

namespace SME.SR.Infra.Dtos.ElasticSearch
{
    [ElasticsearchType(RelationName = "TurmaComponentes")]
    public class TurmaComponentesDto : DocumentoElasticTurma
    {
        [Number(Name = "Modalidade")]
        public int Modalidade { get; set; }
        
        
        [Text(Name = "AnoTurma")]
        public string AnoTurma { get; set; }
        
        
        [Text(Name = "NomeTurma")]
        public string NomeTurma { get; set; }
        
        
        [Text(Name = "ComplementoTurmaEJA")]
        public string ComplementoTurmaEJA { get; set; }
        
        
        [Text(Name = "Turno")]
        public string Turno { get; set; }
        
        
        [Number(Name = "TipoTurma")]
        public int TipoTurma { get; set; }
        
        
        [Boolean(Name = "Historica")]
        public bool Historica { get; set; }
        
        
        [Number(Name = "TipoEscola")]
        public int TipoEscola { get; set; }
        
        
        [Text(Name = "SituacaoTurmaEscola")]
        public string SituacaoTurmaEscola { get; set; }
        
        
        [Date(Name = "DataStatusTurmaEscola", Format = "MMddyyyy")]
        public DateTime DataStatusTurmaEscola { get; set; }
        
        public IEnumerable<ComponenteTurmaDto> Componentes { get; set; }
        
        
        [Number(Name = "EtapaEnsino")]
        public int EtapaEnsino { get; set; }


        [Number(Name = "TipoGradePrograma")]
        public int TipoGradePrograma { get; set; }

        
        [Number(Name = "CodigoGradePrograma")]
        public int CodigoGradePrograma { get; set; }

        
        [Text(Name = "DescricaoGradePrograma")]
        public string DescricaoGradePrograma { get; set; }

        
        [Text(Name = "SerieEnsino")]
        public string SerieEnsino { get; set; }

        
        [Text(Name = "NomeFiltro")]
        public string NomeFiltro { get; set; }
        
        
        [Date(Name = "DataInicioTurma", Format = "MMddyyyy")]
        public DateTime? DataInicioTurma { get; set; }
        
        
        [Date(Name = "DataFimTurma", Format = "MMddyyyy")]
        public DateTime? DataFimTurma { get; set; }
        
        
        [Number(Name = "CicloEnsino")]
        public int CicloEnsino { get; set; }

        
        [Number(Name = "Semestre")]
        public int Semestre { get; set; }
        
        
        [Number(Name = "DuracaoTurno")]
        public int DuracaoTurno { get; set; }
        
        
        [Date(Name = "DataAtualizacao", Format = "MMddyyyy")]
        public DateTime? DataAtualizacao { get; set; }
        
        
        [Number(Name = "EtapaEja")]
        public int EtapaEja { get; set; }

        
        [Number(Name = "EnsinoEspecial")]
        public int EnsinoEspecial { get; set; }

        
        [Number(Name = "Extinta")]
        public int Extinta { get; set; }
    }
}
