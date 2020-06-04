using System;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using SME.SR.Workers.SGP.Commons.Interfaces.Repositories;
using SME.SR.Workers.SGP.Models;
using SME.SR.Infra.Enumeradores;
using SME.SR.JRSClient.Services;
using SME.SR.Infra.Dtos;

namespace SME.SR.Workers.SGP.Commands
{
    public class RelatorioDadosAlunoCommand : IRequest<bool>
    {
        public string FiltroExemplo { get; set; }

        public RelatorioDadosAlunoCommand(string filtroExemplo)
        {
            this.FiltroExemplo = filtroExemplo;
        }
    }

    public class RelatorioDadosAlunoCommandHandler : IRequestHandler<RelatorioDadosAlunoCommand, bool>
    {
        private IEolRepository _sgpRepository;

        public RelatorioDadosAlunoCommandHandler(IEolRepository sgpRepository)
        {
            this._sgpRepository = sgpRepository;
        }

        public async Task<bool> Handle(RelatorioDadosAlunoCommand request, CancellationToken cancellationToken)
        {
            // List <DadosAluno> dadosAluno = await this._sgpRepository.ObterDadosAlunos();

            List<DadosAluno> dadosAluno = new List<DadosAluno> {};
            // TODO move to DI and configurations
            JRSClient.Configuracoes Settings = new JRSClient.Configuracoes
            {
                JasperLogin = "user",
                JasperPassword = "bitnami",
                UrlBase = "http://localhost:8080"
            };

            RelatorioService relatorioService = new RelatorioService(Settings);

            var dados = new Dictionary<string, string>();
            dados.Add("filtro", "json");
            
            var relatorio = await relatorioService.GetRelatorioSincrono(new RelatorioSincronoDto
            {
                CaminhoRelatorio = "/testes/jrsclient/abstract_book_cover.jrxml/abstract_book_cover.jrxml",
                Formato = Enumeradores.FormatoEnum.Pdf,
                ControlesDeEntrada = dados
            }); ;

            return true;
        }
    }
}
