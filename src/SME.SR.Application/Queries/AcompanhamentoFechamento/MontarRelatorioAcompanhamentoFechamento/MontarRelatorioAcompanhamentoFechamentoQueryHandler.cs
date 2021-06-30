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
    public class MontarRelatorioAcompanhamentoFechamentoQueryHandler : IRequestHandler<MontarRelatorioAcompanhamentoFechamentoQuery, RelatorioAcompanhamentoFechamentoPorUeDto>
    {
        public async Task<RelatorioAcompanhamentoFechamentoPorUeDto> Handle(MontarRelatorioAcompanhamentoFechamentoQuery request, CancellationToken cancellationToken)
        {
            var relatorio = new RelatorioAcompanhamentoFechamentoPorUeDto();

            MontarCabecalho(relatorio, request.Dre, request.Ue, request.TurmaCodigo, request.Turmas, request.Usuario);

            return await Task.FromResult(relatorio);
        }

        private void MontarCabecalho(RelatorioAcompanhamentoFechamentoPorUeDto relatorio, Dre dre, Ue ue, string turmaCodigo, IEnumerable<Turma> turmas, Usuario usuario)
        {
            Turma turma = null;

            if (!string.IsNullOrEmpty(turmaCodigo))
                turma = turmas.FirstOrDefault();


            relatorio.Data = DateTime.Now.ToString("dd/MM/yyyy");
            relatorio.DreNome = dre != null ? dre.Nome : "TODAS";
            relatorio.UeNome = ue != null ? ue.Nome : "TODAS";
            relatorio.Turma = turma != null ? turma.NomeRelatorio : "TODAS";
            relatorio.Usuario = usuario.Nome;
            relatorio.RF = usuario.CodigoRf;
        }
    }
}
