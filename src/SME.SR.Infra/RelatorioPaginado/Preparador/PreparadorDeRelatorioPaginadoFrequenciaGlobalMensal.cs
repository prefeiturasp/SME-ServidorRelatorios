using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SR.Infra
{
    public class PreparadorDeRelatorioPaginadoFrequenciaGlobalMensal
    {
        private readonly List<FrequenciaMensalDto> frequenciaGlobalDtos;
        private readonly FrequenciaMensalCabecalhoDto cabecalho;
        public PreparadorDeRelatorioPaginadoFrequenciaGlobalMensal(List<FrequenciaMensalDto> frequenciaGlobalDtos,
            FrequenciaMensalCabecalhoDto cabecalho)
        {
            this.frequenciaGlobalDtos = frequenciaGlobalDtos;
            this.cabecalho = cabecalho;
        }

        public RelatorioPaginadoDto ObtenhaRelatorioPaginadoDto()
        {
            return new RelatorioPaginadoDto()
            {
                Cabecalho = ObtenhaCabecalho(),
                Paginas = ObtenhaPaginaDoRelatorio(),
                ViewCustomConteudo = "FrequenciaGlobalMensal/Conteudo.cshtml",
                ViewCustomParametroCabecalho = "FrequenciaGlobalMensal/Cabecalho.cshtml",
                ViewCustomCss = "FrequenciaGlobalMensal/ConteudoCSS.cshtml"
            };
        }

        private CabecalhoPaginadoDto ObtenhaCabecalho()
        {
            var sondagemCabecalho = this.cabecalho;

            return new CabecalhoPaginadoDto()
            {
                AnoLetivo = sondagemCabecalho.AnoLetivo,
                Dre = sondagemCabecalho.NomeDre,
                NomeRelatorio = "RELATÓRIO DE FREQUÊNCIA GLOBAL MENSAL",
                Rf = sondagemCabecalho.RfUsuarioSolicitante,
                Turma = sondagemCabecalho.NomeTurma,
                Ue = sondagemCabecalho.NomeUe,
                Usuario = sondagemCabecalho.UsuarioSolicitante,
                Modalidade = sondagemCabecalho.NomeModalidade,
                MesReferencia = sondagemCabecalho.MesReferencia,
            };
        }
        private List<Pagina> ObtenhaPaginaDoRelatorio()
        {
            var lista = new List<Pagina>();

            lista.AddRange(ObtenhaPaginas());
            return lista;
        }
        private List<Pagina> ObtenhaPaginas()
        {
            var parametro = new ParametroRelatorioPaginadoPorColuna<FrequenciaMensalDto>()
            {
                AlturaDaLinha = 25,
                TipoDePapel = TipoPapel.A4,
                UnidadeDeTamanho = EnumUnidadeDeTamanho.PERCENTUAL,
                Valores = this.frequenciaGlobalDtos
            };
            var relatorioPaginado = new RelatorioPaginadoFrequenciaMensal(parametro, ObtenhaDicionarioColuna(), this.cabecalho);

            relatorioPaginado.AdicioneAgrupamento(grupo => grupo.CodigoDre);
            relatorioPaginado.AdicioneAgrupamento(grupo => grupo.CodigoUe);
            relatorioPaginado.AdicioneAgrupamento(grupo => grupo.CodigoTurma);
            relatorioPaginado.AdicioneAgrupamento(grupo => grupo.ValorMes);

            return relatorioPaginado.Paginas();
        }
        private List<IColuna> ObtenhaDicionarioColuna()
        {
            var lista = new List<IColuna>();

            lista.Add(new Coluna<FrequenciaMensalDto>()
            {
                Largura = 6,
                Titulo = "Nº",
                Nome = "NumeroAluno",
                UnidadeDeTamanho = EnumUnidadeDeTamanho.PERCENTUAL
            });
            lista.Add(new Coluna<FrequenciaMensalDto>()
            {
                Largura = 86,
                Titulo = "NOME",
                Nome = "NomeAluno",
                UnidadeDeTamanho = EnumUnidadeDeTamanho.PERCENTUAL
            });
            lista.Add(new Coluna<FrequenciaMensalDto>()
            {
                Largura = 8,
                Titulo = "% FREQ",
                Nome = "ProcentagemFrequencia",
                UnidadeDeTamanho = EnumUnidadeDeTamanho.PERCENTUAL
            });

            return lista;
        }
    }
}
