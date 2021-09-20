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
            MontarCabecalho(relatorio, request.Dre, request.Ue, request.Bimestres, request.Usuario);
            if (request.Bimestres == null || !request.Bimestres.Any())
            {
                var bimestresFechamento = request.ConsolidadoFechamento.Select(f => f.Bimestre);
                var bimestresConselho = request.ConsolidadoConselhosClasse.Select(f => f.Bimestre);

                var bimestres = new List<int>();
                bimestres.AddRange(bimestresFechamento);
                bimestres.AddRange(bimestresConselho);

                bimestres = bimestres.Distinct().OrderBy(b => b == 0).ThenBy(b => b).ToList();

                request.Bimestres = bimestres.ToArray();
            }
            else
                request.Bimestres = request.Bimestres.OrderBy(b => b == 0).ThenBy(b => b).ToArray();


            foreach (var bimestre in request.Bimestres)
            {
                var listaConsolidadoFechamento = request.ConsolidadoFechamento.GroupBy(x => x.UeCodigo).ToList();
                var listaConsolidadoConselhoClasse = request.ConsolidadoConselhosClasse.GroupBy(x => x.UeCodigo).ToList();
                foreach (var fechamento in listaConsolidadoFechamento)
                {
                    var agrupadoPorBimestre = fechamento.GroupBy(x =>x.Bimestre);
                    foreach (var agrupado in agrupadoPorBimestre)
                    {
                        var agrupadoPorTurma = agrupado.GroupBy(x => x.TurmaCodigo);
                        agrupadoPorTurma.
                    }
                }
                var nomeUe = await ObterNomeUe(ue.Key);
                var uesRelatorio = new RelatorioAcompanhamentoFechamentoConsolidadoUesDto(nomeUe);
                var bimestreAgrupado = ue.GroupBy(x => x.Bimestre);
                foreach (var bimestre in bimestreAgrupado)
                {
                    var nomeBimestre = bimestre.FirstOrDefault().Bimestre == 0 ?
                                       "FINAL"
                                       :
                                       $"{bimestre.FirstOrDefault().Bimestre}º BIMESTRE";

                    var fechamentos = request.ConsolidadoFechamento.Where(f => f.Bimestre == bimestre).GroupBy(x => x.NomeUe);
                    var conselhos = request.ConsolidadoConselhosClasse.Where(f => f.Bimestre == bimestre).GroupBy(x => x.NomeUe);
                    var bimestres = new RelatorioAcompanhamentoFechamentoConsolidadoBimestresDto(nomeBimestre, ltsUes.TurmaCodigo);

                    foreach (var fechamento in fechamentos)
                    {
                        foreach (var fech in fechamento)
                        {
                            var fechamentoCon = new RelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoDto(fech.NomeTurmaFormatado);
                            var fechamentoConsolidado = new RelatorioAcompanhamentoFechamentoConsolidadoDto
                            {
                                NaoIniciado = fech.NaoIniciado,
                                ProcessadoComPendencia = fech.ProcessadoComPendencia,
                                ProcessadoComSucesso = fech.ProcessadoComPendencia
                            };
                            fechamentoCon.FechamentoConsolidado = fechamentoConsolidado;
                            foreach (var conselho in conselhos)
                            {
                                foreach (var cons in conselho)
                                {
                                    var conselhoClasseConsolidado = new RelatorioAcompanhamentoConselhoClasseConsolidadoDto
                                    {
                                        NaoIniciado = cons.NaoIniciado,
                                        EmAndamento = cons.EmAndamento,
                                        Concluido = cons.Concluido
                                    };
                                    fechamentoCon.ConselhoDeClasseConsolidado = conselhoClasseConsolidado;
                                }
                            }
                            bimestres.FechamentoConselhoClasseConsolidado.Add(fechamentoCon);
                        }
                    }
                    if (bimestres?.FechamentoConselhoClasseConsolidado.Count() > 0)
                        uesRelatorio.Bimestres.Add(bimestres);
                }


                if (uesRelatorio?.Bimestres?.Count() > 0)
                    relatorio.Ues.Add(uesRelatorio);
            }
            return await Task.FromResult(relatorio);


        }

        private async Task<string> ObterNomeUe(string ueCodigo)
        {
            var ue = await mediator.Send(new ObterUePorCodigoQuery(ueCodigo));

            return ue.TituloTipoEscolaNome;
        }

        private static IEnumerable<FechamentoConsolidadoTurmaDto> MaperarUePorTurma(MontasRelatorioAcompanhamentoFechamentoConselhoClasseConsolidadoQuery request)
        {
            var dto = new List<FechamentoConsolidadoTurmaDto>();
            var retornoDto = new List<FechamentoConsolidadoTurmaDto>();

            //foreach (var bimestre in request.Bimestres)
            //{
            //    var ues = request.ConsolidadoFechamento?.Where(x => x.TurmaCodigo == turma.Codigo && x.Bimestre == bimestre);
            //    foreach (var ue in ues)
            //    {
            //        dto.Add(ue);
            //    }
            //}

            //foreach (var agrupado in dto.GroupBy(x => x.Bimestre).ToList())
            //{
            //    var listaAgrupada = agrupado.ToArray();
            //    foreach (var ltsAgrupado in listaAgrupada)
            //    {
            //        retornoDto.Add(ltsAgrupado);
            //    }
            //}
            return retornoDto;
        }

        private void MontarCabecalho(RelatorioAcompanhamentoFechamentoConsolidadoPorUeDto relatorio, Dre dre, Ue ue, int[] bimestres, Usuario usuario)
        {
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

            relatorio.Bimestre = bimestre;
            relatorio.Data = DateTime.Now.ToString("dd/MM/yyyy");
            relatorio.DreNome = dre != null ? dre.Abreviacao : "TODAS";
            relatorio.UeNome = ue != null ? ue.NomeRelatorio : "TODAS";
            relatorio.Turma = turma;
            relatorio.Usuario = usuario.Nome;
            relatorio.RF = usuario.CodigoRf;
        }
    }
}
