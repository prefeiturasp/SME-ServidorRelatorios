using Microsoft.Extensions.DependencyInjection;
using SME.SR.Application.Interfaces;
using SME.SR.Infra;
using System;
using System.Collections.Generic;

namespace SME.SR.Application.Services
{
    public class FabricaDeServicoAnaliticoSondagem : IFabricaDeServicoAnaliticoSondagem
    {
        private const int TERCEIRO_BIMESTRE = 3;
        private const int SEGUNDO_SEMESTRE = 2;
        private const int ANO_LETIVO_2024 = 2024;
        private const int ANO_LETIVO_2025 = 2025;
        private FiltroRelatorioAnaliticoSondagemDto filtro;
        private IDictionary<TipoSondagem, Func<IServicoRepositorioAnalitico>> dicionarioServico;
        private readonly IServiceScopeFactory servicos;

        public FabricaDeServicoAnaliticoSondagem(IServiceScopeFactory servicos)
        {
            this.servicos = servicos ?? throw new System.ArgumentNullException(nameof(servicos));
            this.dicionarioServico = ObterServicos();
        }

        public IServicoRepositorioAnalitico CriarServico(FiltroRelatorioAnaliticoSondagemDto filtro)
        {
            this.filtro = filtro;

            if (dicionarioServico.ContainsKey(filtro.TipoSondagem))
                return dicionarioServico[filtro.TipoSondagem]();

            return null;
        }

        private IDictionary<TipoSondagem, Func<IServicoRepositorioAnalitico>> ObterServicos()
        {
            return new Dictionary<TipoSondagem, Func<IServicoRepositorioAnalitico>>()
            {
                { TipoSondagem.LP_CapacidadeLeitura, ObterServicoCapacidadeDeLeitura },
                { TipoSondagem.LP_Leitura, ObterServicoAnaliticoLeitura },
                { TipoSondagem.LP_LeituraVozAlta, ObterServicoLeituraVozAlta },
                { TipoSondagem.LP_Escrita, ObterServicoEscrita },
                { TipoSondagem.LP_ProducaoTexto, ObterServicoProducaoDeTexto },
                { TipoSondagem.MAT_CampoAditivo, ObterServicoProducaoCampoAditivo },
                { TipoSondagem.MAT_CampoMultiplicativo, ObterServicoProducaoCampoMultiplicativo },
                { TipoSondagem.MAT_Numeros, ObterServicoProducaoCampoNumero },
                { TipoSondagem.MAT_IAD, ObterServicoProducaoMatematicaIAD }
            };
        }

        private IServicoRepositorioAnalitico ObterInstancia(Type tipo)
        {
            using var scope = servicos.CreateScope();
            return (IServicoRepositorioAnalitico)scope.ServiceProvider.GetService(tipo);
        }

        private IServicoRepositorioAnalitico ObterServicoCapacidadeDeLeitura()
        {
            var tipoServico = ComPreenchimentoDeTodosEstudantesIAD() ?
                                    typeof(IServicoAnaliticoSondagemCapacidadeDeLeituraTodosPreenchido) :
                                    typeof(IServicoAnaliticoSondagemCapacidadeDeLeitura);

            return ObterInstancia(tipoServico);
        }

        private IServicoRepositorioAnalitico ObterServicoAnaliticoLeitura()
        {
            return null;
        }

        private IServicoRepositorioAnalitico ObterServicoEscrita()
        {
            return null;
        }

        private IServicoRepositorioAnalitico ObterServicoLeituraVozAlta()
        {
            return null;
        }
        
        private IServicoRepositorioAnalitico ObterServicoProducaoDeTexto()
        {
            return null;
        }

        private IServicoRepositorioAnalitico ObterServicoProducaoCampoAditivo()
        {
            return null;
        }

        private IServicoRepositorioAnalitico ObterServicoProducaoCampoMultiplicativo()
        {
            return null;
        }

        private IServicoRepositorioAnalitico ObterServicoProducaoCampoNumero()
        {
            return null;
        }

        private IServicoRepositorioAnalitico ObterServicoProducaoMatematicaIAD()
        {
            return null;
        }

        private bool ComPreenchimentoDeTodosEstudantesIAD()
        {
            return filtro.AnoLetivo == ANO_LETIVO_2024 && filtro.Periodo == SEGUNDO_SEMESTRE || filtro.AnoLetivo >= ANO_LETIVO_2025;
        }

        private bool ComPreenchimentoDeTodosEstudantes()
        {
            return filtro.AnoLetivo == ANO_LETIVO_2024 && filtro.Periodo == TERCEIRO_BIMESTRE || filtro.AnoLetivo >= ANO_LETIVO_2025;
        }
    }
}
