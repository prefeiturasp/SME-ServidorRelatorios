using Elasticsearch.Net;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using Org.BouncyCastle.Ocsp;
using SME.SR.Infra.Dtos.Relatorios.MapeamentoEstudante;
using SME.SR.Infra.Utilitarios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.SR.Infra
{
    public class MapeamentoEstudanteUltimoBimestreDto
    {
        public MapeamentoEstudanteUltimoBimestreDto()
        {
            RespostrasBimestrais = new List<RespostaBimestralMapeamentoEstudanteDto>();
        }
        public long Id { get; set; }    
        public long DreCodigo { get; set; }
        public string DreAbreviacao { get; set; }
        public string UeCodigo { get; set; }
        public string UeNome { get; set; }
        public TipoEscola TipoEscola { get; set; }
        public string AlunoCodigo { get; set; }
        public string AlunoNome { get; set; }
        public long TurmaId { get; set; }
        public string TurmaCodigo { get; set; }
        public string TurmaNome { get; set; }
        public Modalidade Modalidade { get; set; }
        public int AnoLetivo { get; set; }
        public int Semestre { get; set; }
        public string ParecerConclusivoAnoAnterior { get; set; }
        public string TurmaAnoAnterior { get; set; }
        public string DistorcaoIdadeAnoSerie { get; set; }
        public string Nacionalidade { get; set; }
        public string AcompanhadoSRMCEFAI { get; set; }
        public string PossuiPlanoAEE { get; set; }
        public string AcompanhadoNAAPA { get; set; }
        public string ParticipaPAP { get; set; }
        public string ParticipaProjetosMaisEducacao { get; set; }
        public string ProjetosFortalecimentoAprendizagem { get; set; }
        public string ProgramaSPIntegral { get; set; }
        public string AvaliacoesExternasProvaSP { get; set; }
        
        public List<RespostaBimestralMapeamentoEstudanteDto> RespostrasBimestrais { get; }
        public string UeCompleta => $"{TipoEscola.ShortName()} {UeNome}";
        public string TurmaCompleta => $"{Modalidade.ShortName()} - {TurmaNome}";
        
        public void AdicionarRespostaBimestral(RespostaBimestralMapeamentoEstudanteDto respostaBimestral)
        {
            if (!RespostrasBimestrais.Any(r => r.Bimestre.Equals(respostaBimestral.Bimestre)))
                RespostrasBimestrais.Add(respostaBimestral);
        }

        public IEnumerable<AvaliacaoExternaProvaSPDto> ObterAvaliacoesExternasProvaSP()
            => JsonConvert.DeserializeObject<IEnumerable<AvaliacaoExternaProvaSPDto>>(AvaliacoesExternasProvaSP).OrderBy(sp => sp.AreaConhecimento);

        public string ObterParecerConclusivoAnoAnterior()
            => string.IsNullOrEmpty(ParecerConclusivoAnoAnterior) 
            ? string.Empty 
            : JsonConvert.DeserializeObject<ItemGenericoJSON>(ParecerConclusivoAnoAnterior).value;

        public string[] ObterProgramasPAP()
            => string.IsNullOrEmpty(ParticipaPAP)
            ? Enumerable.Empty<string>().ToArray()
            : JsonConvert.DeserializeObject<List<ItemGenericoJSON>>(ParticipaPAP).Select(pap => pap.value).ToArray();

        public string[] ObterProgramasMaisEducacao()
            => string.IsNullOrEmpty(ParticipaProjetosMaisEducacao)
            ? Enumerable.Empty<string>().ToArray()
            : JsonConvert.DeserializeObject<List<ItemGenericoJSON>>(ParticipaProjetosMaisEducacao).Select(pap => pap.value).ToArray();

        public string[] ObterProjetosFortalecimentoAprendizagem()
            => string.IsNullOrEmpty(ProjetosFortalecimentoAprendizagem)
            ? Enumerable.Empty<string>().ToArray()
            : JsonConvert.DeserializeObject<List<ItemGenericoJSON>>(ProjetosFortalecimentoAprendizagem).Select(pap => pap.value).ToArray();
    }

    public sealed class ItemGenericoJSON
    {
        public string index { get; set; }
        public string value { get; set; }
    }

    public class RespostaBimestralMapeamentoEstudanteDto
    {
        public int Bimestre { get; set; }
        public string AnotacoesPedagogicasBimestreAnterior_Bimestre { get; set; }
        public string AcoesRedeApoio_Bimestre { get; set; }
        public string AcoesRecuperacaoContinua_Bimestre { get; set; }
        public string HipoteseEscrita_Bimestre { get; set; }
        public string ObsAvaliacaoProcessual_Bimestre { get; set; }
        public string Frequencia_Bimestre { get; set; }
        public string QdadeRegistrosBuscasAtivas_Bimestre { get; set; }
    }
}
