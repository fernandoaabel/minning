using Avaliador_Textual.Models;
using Avaliador_Textual.Models.Outros;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using weka.classifiers;
using weka.classifiers.meta;
using weka.core;
using weka.core.converters;

namespace Avaliador_Textual.Controllers
{
    public class AvaliacaoController : BaseController
    {
        private SiteDB db = new SiteDB();
        
        public ActionResult Index()
        {
            return View();
        }

        // Apreensibilidade
        public ActionResult IndiceFormula()
        {
            return View();
        }

        public ActionResult DificuldadeLeitura()
        {
            return View();
        }

        public ActionResult EscolaridadeAproximada()
        {
            return View();
        }

        public ActionResult WordCloud()
        {            
            return View();
        }

        // Qualidade
        public ActionResult Classificacao()
        {
            return View();
        }

        public ActionResult BubbleChart()
        {
            return View();
        }

        public ActionResult BubbleChartTags()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetFrequenciaPalavrasDB(string tag)
        {
            // Consulta os arquivos classificados na respectiva Tag
            var arquivosTreinamento = db.Arquivos.Where(p => p.Tag.Equals(tag));

            // Compoe um JArray com todas as palavras de todos os arquivos (mesmo que repetidas)
            JArray json = new JArray();
            foreach (var arquivo in arquivosTreinamento)
            {
                JArray v = JArray.Parse(arquivo.FrequenciaPalavras);
                json.Merge(v);
            }

            // Transforma o JSON resultado em uma lista
            List<Token> items = json.Select(x => new Token
            {
                Palavra    = (string)x["Palavra"],
                Frequencia = (int?)x["Frequencia"]
            }).ToList();

            // Agrupa os resultados pela "Palavra"
            var Resultado = items
                            .GroupBy(p => p.Palavra)
                            .Select(p => new Token( p.First().Palavra
                                                  , p.Sum(c => c.Frequencia)
                                                  )
                                   );

            // Descobre o maior valor
            var max = Resultado.Max(item => item.Frequencia);
            // Frequencia minima corresponde a 5% do maior valor
            double frequenciaMinima = (double) max * 5 / 100;
            frequenciaMinima = Math.Round(frequenciaMinima);

            Resultado = Resultado.Where(p => p.Frequencia > frequenciaMinima).ToList();

            var jsonResult = Json(Resultado, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
        
        [HttpGet]
        public JsonResult GetDistinctTag()
        {
            var tags = db.Arquivos.Select(m => m.Tag).Distinct();

            return Json(tags, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Resultados(string texto, string nome)
        {
            if (texto == null && nome == null)
            {
                TempData["Message"] = "Necessário informar um texto ou URL de site para ser avaliado.";
                return RedirectToAction("Index");
            }
            else if (!nome.Equals(""))
            {
                // Valida se a URL é válida
                if (!Utilidades.CheckURLValid(nome))
                {
                    TempData["Message"] = "A URL informada não é válida ou está indisponível.";
                    return RedirectToAction("Index");
                }
            }
            
            // Instancia o novo arquivo
            Arquivo ArquivoAvaliado = new Arquivo(texto, nome);
            ArquivoAvaliado.InicializaArquivo();

            try
            {
                ArquivoAvaliado = AvaliarWekaClassificacao(ArquivoAvaliado);
            } catch (Exception e)
            {
                TempData["Message"] = e.Message;
            }

            return View(ArquivoAvaliado);
        }

        // Busca o Modelo salvo na pasta Classificador do Projeto
        private Classifier ImportaClassificadorSalvo()
        {
            Classifier classifier;
            try
            {
                classifier = (Classifier)SerializationHelper.read(string.Format("{0}Classificador.model", DiretorioClassificadorServidor));
            }
            catch (Exception)
            {
                throw (new ClassificationException("Nenhum classificador foi treinado para realizar essa avaliação."));
            }
            
            return classifier;
        }

        private Instances ImportaDadosTreinamentoSalvo()
        {
            // Busca os dados de treinamentos salvos na pasta /Classificador/
            ArffLoader loader = new ArffLoader();
            loader.setFile(new java.io.File(string.Format("{0}DadosTreinamento.arff", DiretorioClassificadorServidor)));
            return loader.getDataSet();
        }

        private Arquivo AvaliarWekaClassificacao(Arquivo _arquivoAvaliado)
        {
            // Importa o Classificador que foi salvo pelo treinamento
            FilteredClassifier classifier = (FilteredClassifier)ImportaClassificadorSalvo();

            // Importa os Dados de Treinamento salvo pelo treinamento
            Instances dadosTreinamento = ImportaDadosTreinamentoSalvo();
            dadosTreinamento.setClassIndex(dadosTreinamento.numAttributes()-1); // Classe fica em último

            int numAttributes = dadosTreinamento.numAttributes();

            // Cria a instância de teste
            Instance instance = new DenseInstance(numAttributes);
            instance.setDataset(dadosTreinamento);

            // Inicializa todos os atributos como valor zero.
            for (int i = 0; i < numAttributes; i++)
                instance.setValue(i, 0);

            // Insere o texto a ser avaliado no primeiro atributo
            for (int i = 0; i < numAttributes-1; i++)
            {
                instance.setValue(i, _arquivoAvaliado.TextoFormatado);
            }
            
            // Indica que a Classe está faltando, para que a mesma possa ser classificada
            instance.setClassMissing();
            
            // Classifica a instância de teste
            var resultado = "";
            try
            {
                // Realiza a classificação da instância, retornando o resultado previsto
                var predicao = classifier.classifyInstance(instance);
                instance.setClassValue(predicao);
                // Realiza a tradução do resultado numérico na Classificação esperada
                resultado = dadosTreinamento.classAttribute().value((int) predicao);

                var distribuicao = classifier.distributionForInstance(instance);

            } catch (Exception)
            {
                throw (new ClassificationException("O texto não pode ser classificado quanto à sua qualidade."));
            }
            
            // Atribui o resultado ao arquivo avaliado
            _arquivoAvaliado.Classificacao = resultado;

            return _arquivoAvaliado;
        }
    }
}