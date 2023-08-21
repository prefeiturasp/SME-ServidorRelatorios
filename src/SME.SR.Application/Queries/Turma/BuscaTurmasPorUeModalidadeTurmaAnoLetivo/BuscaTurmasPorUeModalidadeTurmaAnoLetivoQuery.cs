using System;
using MediatR;
using SME.SR.Infra.Dtos;

namespace SME.SR.Application.Queries
{
    public class BuscaTurmasPorUeModalidadeTurmaAnoLetivoQuery : IRequest<PaginacaoResultadoDto<RetornoConsultaListagemTurmaComponenteDto>>
    {
        public BuscaTurmasPorUeModalidadeTurmaAnoLetivoQuery(string codigoUe, int modalidade,long codigoTurma,int anoLetivo,
            int qtdeRegistros,int qtdeRegistrosIgnorados,bool ehProfessor, string codigoRf,bool consideraHistorico,
            DateTime periodoEscolarInicio,string[] anosInfantilDesconsiderar)
        {
            CodigoUe = codigoUe;
            Modalidade = modalidade;
            Modalidade = modalidade;
            CodigoTurma = codigoTurma;
            AnoLetivo = anoLetivo;
            QtdeRegistros = qtdeRegistros;
            QtdeRegistrosIgnorados = qtdeRegistrosIgnorados;
            EhProfessor = ehProfessor;
            CodigoRf = codigoRf;
            ConsideraHistorico = consideraHistorico;
            PeriodoEscolarInicio = periodoEscolarInicio;
            AnosInfantilDesconsiderar = anosInfantilDesconsiderar;
        }
        public string CodigoUe { get; set; }
        public int Modalidade { get; set; }
        public long CodigoTurma { get; set; }
        public int AnoLetivo { get; set; }
        public int QtdeRegistros { get; set; }
        public int QtdeRegistrosIgnorados { get; set; }
        public bool EhProfessor { get; set; }
        public string CodigoRf { get; set; }
        public bool ConsideraHistorico { get; set; }
        public DateTime PeriodoEscolarInicio { get; set; }
        public string[] AnosInfantilDesconsiderar { get; set; }
    }
}