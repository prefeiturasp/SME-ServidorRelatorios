using MediatR;
using SME.SR.Data;
using SME.SR.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SME.SR.Application
{
    public class MontasRelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoQueryHandler : IRequestHandler<MontasRelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoQuery, RelatorioAcompanhamentoFechamentoConsolidadoPorUeDto>
    {
        private readonly IMediator mediator;

        public MontasRelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoQueryHandler(IMediator mediator)
        {
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        public async Task<RelatorioAcompanhamentoFechamentoConsolidadoPorUeDto> Handle(MontasRelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoQuery request, CancellationToken cancellationToken)
        {
            var relatorio = new RelatorioAcompanhamentoFechamentoConsolidadoPorUeDto();
            var relatorioUe = new RelatorioAcompanhamentoFechamentoUesDto();
            relatorio.Cabecalho = MontarCabecalho(request.Dre, request.Ue, request.Bimestres, request.Usuario);

            foreach(var ue in request.ConsolidadoFechamento.GroupBy(u => u.NomeUe).OrderBy(x => x.Key))
            {
                relatorio.Ues.Add(await MapearParaUe(ue, request.ConsolidadoConselhosClasse));                
            };
            
            return await Task.FromResult(relatorio);
        }

        private async Task<string> ObterNomeUe(string ueCodigo)
        {
            var ue = await mediator.Send(new ObterUePorCodigoQuery(ueCodigo));
            if (ue == null)
                return string.Empty;

            return ue.TituloTipoEscolaNome;
        }       

        private RelatorioAcompanhamentoFechamentoCabecalhoDto MontarCabecalho(Dre dre, Ue ue, int[] bimestres, Usuario usuario)
        {
            var cabecalho = new RelatorioAcompanhamentoFechamentoCabecalhoDto();
            string turma = "TODAS";
            string bimestre = "TODOS";

            if (bimestres != null && bimestres.Any())
            {
                if (bimestres.Contains(0))
                {
                    var strBimestres = bimestres.Select(x => x.ToString()).ToList();

                    for (int i = 0; i < strBimestres.Count(); i++)
                    {
                        if (strBimestres[i].Equals("0"))
                            strBimestres[i] = "FINAL";
                        else
                            strBimestres[i] = $"{strBimestres[i]}º";
                    }

                    strBimestres = strBimestres.OrderBy(b => b).ToList();

                    bimestre = string.Join(",", strBimestres);
                }
                else if (bimestres.Count() == 1)
                    bimestre = $"{bimestres.FirstOrDefault()}º";
                else
                    bimestre = string.Join(", ", bimestres.Select(b => $"{b}º").OrderBy(b => b));
            }

            cabecalho.Bimestre = bimestre;            
            cabecalho.DreNome = dre != null ? dre.Abreviacao : "TODAS";
            cabecalho.UeNome = ue != null ? ue.NomeRelatorio : "TODAS";
            cabecalho.Turma = turma;
            cabecalho.Usuario = usuario.Nome;
            cabecalho.RF = usuario.CodigoRf;

            return cabecalho;
        }

        private async Task<RelatorioAcompanhamentoFechamentoUesDto> MapearParaUe(IGrouping<string, FechamentoConsolidadoTurmaDto> fechamentoTurma, IEnumerable<ConselhoClasseConsolidadoTurmaDto> conselhosClasseTurma)
        {
            var UeFechamento = new RelatorioAcompanhamentoFechamentoUesDto();
            UeFechamento.NomeUe = await ObterNomeUe(fechamentoTurma.FirstOrDefault().UeCodigo);

            foreach (var bimestre in fechamentoTurma.GroupBy(c => c.Bimestre).OrderBy(d => d.Key == 0).ThenBy(d => d.Key))            
                UeFechamento.Bimestres.Add(await MapearParaBimestre(bimestre, conselhosClasseTurma));
            
            return UeFechamento;
        }


        private async Task<RelatorioAcompanhamentoFechamentoBimestresDto> MapearParaBimestre(IGrouping<int, FechamentoConsolidadoTurmaDto> fechamentoTurma, IEnumerable<ConselhoClasseConsolidadoTurmaDto> conselhosClasseTurma)
        {
            var bimestreFechamento = new RelatorioAcompanhamentoFechamentoBimestresDto();

            bimestreFechamento.Bimestre = fechamentoTurma.FirstOrDefault().Bimestre == 0 ?
                                                 $"Bimestre Final"
                                                 :
                                                 $"{fechamentoTurma.FirstOrDefault().Bimestre}º Bimestre";

            foreach (var turmaFechamento in fechamentoTurma.OrderBy(t => t.NomeTurma))
                bimestreFechamento.Turmas.Add(MapearParaTurma(turmaFechamento, conselhosClasseTurma));

            return await Task.FromResult(bimestreFechamento);
        }

        private RelatorioAcompanhamentoFechamentoConselhoClasseDto MapearParaTurma(FechamentoConsolidadoTurmaDto fechamentoTurma, IEnumerable<ConselhoClasseConsolidadoTurmaDto> conselhosClasseTurma)
        {
            var turma = new RelatorioAcompanhamentoFechamentoConselhoClasseDto();
            turma.NomeTurma = fechamentoTurma.NomeTurmaFormatado;

            turma.FechamentoConsolidado.NaoIniciado = fechamentoTurma.NaoIniciado;
            turma.FechamentoConsolidado.ProcessadoComPendencia = fechamentoTurma.ProcessadoComPendencia;
            turma.FechamentoConsolidado.ProcessadoComSucesso = fechamentoTurma.ProcessadoComSucesso;

            var conselhoClasseFiltrado = conselhosClasseTurma?.FirstOrDefault(c => c.TurmaCodigo == fechamentoTurma.TurmaCodigo && c.Bimestre == fechamentoTurma.Bimestre);
            turma.ConselhoDeClasseConsolidado.NaoIniciado = conselhoClasseFiltrado != null ? conselhoClasseFiltrado.NaoIniciado : 0;
            turma.ConselhoDeClasseConsolidado.EmAndamento = conselhoClasseFiltrado != null ? conselhoClasseFiltrado.EmAndamento: 0;
            turma.ConselhoDeClasseConsolidado.Concluido = conselhoClasseFiltrado != null ? conselhoClasseFiltrado.Concluido : 0;

            return turma;
        }
    }
}
