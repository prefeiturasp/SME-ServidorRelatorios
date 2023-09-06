using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SME.SR.Infra
{
    public class EncaminhamentoNaapaPaginado
    {
        private RelatorioEncaminhamentosNAAPADto relatorioEncaminhamentosNAAPADto;
        private List<RelatorioEncaminhamentosNAAPADto> Paginas;

        private const int TOTAL_LINHAS = 32;
        private const int TOTAL_COLUNA_TURMA = 15;
        private const int TOTAL_COLUNA_ALUNO = 52;
        private const int TOTAL_COLUNA_PORTA = 29;
        private const int TOTAL_COLUNA_SITUACAO = 29;
        private const int QTDE_CHARS_POR_LINHA = 93;
        private const string TODAS = "TODAS";

        private bool MostrarDre { get; set; } 
        private bool MostrarUe { get; set; }  

        public EncaminhamentoNaapaPaginado(RelatorioEncaminhamentosNAAPADto relatorioEncaminhamentosNAAPADto)
        {
            this.relatorioEncaminhamentosNAAPADto = relatorioEncaminhamentosNAAPADto;
            MostrarDre = relatorioEncaminhamentosNAAPADto.DreNome == TODAS;
            MostrarUe = relatorioEncaminhamentosNAAPADto.UeNome == TODAS;
            Paginas = new List<RelatorioEncaminhamentosNAAPADto>();
        }

        public List<RelatorioEncaminhamentosNAAPADto> ObterPaginas()
        {
            ExecutarPaginacao();

            return Paginas;
        }

        private void ExecutarPaginacao()
        {
            var linhas = 0;
            var paginaAtual = ObterPagina();

            foreach (var dto in relatorioEncaminhamentosNAAPADto.EncaminhamentosDreUe)
            {
                var agrupamentoAtual = ObterAgrupamento(dto);
                paginaAtual.EncaminhamentosDreUe.Add(agrupamentoAtual);

                foreach (var detalhe in dto.Detalhes)
                {
                    var linhasPorDetalhe = ObterLinhasDeQuebra(detalhe);

                    linhas += linhasPorDetalhe;

                    if (linhas >= TOTAL_LINHAS)
                    {
                        var mostrarAgrupamento = RemoveuAgrupamentoSemDetalhe(paginaAtual);
                        AdicionarPagina(paginaAtual);
                        linhas = linhasPorDetalhe;
                        paginaAtual = ObterPagina();
                        agrupamentoAtual = ObterAgrupamento(dto, mostrarAgrupamento);
                        paginaAtual.EncaminhamentosDreUe.Add(agrupamentoAtual);
                    }

                    agrupamentoAtual.Detalhes.Add(detalhe);
                }

                if (agrupamentoAtual.Detalhes.Any() && linhas < TOTAL_LINHAS)
                {
                    var mostrarAgrupamento = RemoveuAgrupamentoSemDetalhe(paginaAtual);
                    AdicionarPagina(paginaAtual);
                    paginaAtual = ObterPagina();
                    agrupamentoAtual = ObterAgrupamento(dto, mostrarAgrupamento);
                    paginaAtual.EncaminhamentosDreUe.Add(agrupamentoAtual);
                }

                linhas += MostraAgrupamento() ? 2 : 0;
            }

            if (linhas > 0 && Paginas.Count == 0)
                AdicionarPagina(paginaAtual);
        }

        private bool MostraAgrupamento()
        {
            return MostrarDre || MostrarUe;
        }

        private bool RemoveuAgrupamentoSemDetalhe(RelatorioEncaminhamentosNAAPADto pagina)
        {
            var grupo = pagina.EncaminhamentosDreUe.LastOrDefault();

            if (grupo != null && !grupo.Detalhes.Any())
                return pagina.EncaminhamentosDreUe.Remove(grupo);

            return false;
        }

        private AgrupamentoEncaminhamentoNAAPADreUeDto ObterAgrupamento(AgrupamentoEncaminhamentoNAAPADreUeDto dto, bool mostrar = true)
        {
            return new AgrupamentoEncaminhamentoNAAPADreUeDto()
            {
                DreId = dto.DreId,
                DreNome = dto.DreNome,
                UeNome = dto.UeNome,
                UeOrdenacao = dto.UeOrdenacao,
                MostrarAgrupamento = mostrar,
                Detalhes = new List<DetalheEncaminhamentoNAAPADto>()
            };
        }

        private void AdicionarPagina(RelatorioEncaminhamentosNAAPADto pagina)
        {
            Paginas.Add(pagina);
        }

        private RelatorioEncaminhamentosNAAPADto ObterPagina()
        {
            return new RelatorioEncaminhamentosNAAPADto()
            {
                DreNome = relatorioEncaminhamentosNAAPADto.DreNome,
                UeNome = relatorioEncaminhamentosNAAPADto.UeNome,
                UsuarioNome = relatorioEncaminhamentosNAAPADto.UsuarioNome,
                EncaminhamentosDreUe = new List<AgrupamentoEncaminhamentoNAAPADreUeDto>()
            };
        }

        private int ObterLinhasDeQuebra(DetalheEncaminhamentoNAAPADto detalhe)
        {
            var funcoesLimiteCaracteres = ObterFuncoesLimiteCaracteres();
            
            return 3 + funcoesLimiteCaracteres.Count(funcao => funcao(detalhe)) + ObterLinhasFluxoDeAlerta(detalhe);
        }

        private IEnumerable<Func<DetalheEncaminhamentoNAAPADto, bool>> ObterFuncoesLimiteCaracteres()
        {
            return new List<Func<DetalheEncaminhamentoNAAPADto, bool>>
            {
                detalhe => detalhe.PortaEntrada.Length > TOTAL_COLUNA_PORTA,
                detalhe => detalhe.Situacao.Length > TOTAL_COLUNA_SITUACAO,
                detalhe => detalhe.Aluno.Length > TOTAL_COLUNA_ALUNO || detalhe.Turma.Length > TOTAL_COLUNA_TURMA
            };
        }

        private int ObterLinhasFluxoDeAlerta(DetalheEncaminhamentoNAAPADto dto)
        {
            var qtdeLinha = 1;
            var qtde = Math.Round((double)dto.FluxosAlerta.Length / QTDE_CHARS_POR_LINHA);
            if (qtde > 1) qtdeLinha = (int)qtde;

            return qtdeLinha;
        }
    }
}
