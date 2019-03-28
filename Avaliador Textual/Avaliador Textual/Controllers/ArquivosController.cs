using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Avaliador_Textual.Models;
using System.IO;
using Avaliador_Textual.Models.Outros;
using weka.core.converters;
using weka.core;
using weka.filters.unsupervised.attribute;
using System;
using weka.classifiers.bayes;
using weka.classifiers.meta;
using weka.filters.supervised.attribute;
using weka.classifiers;

namespace Avaliador_Textual.Controllers
{
    [Authorize]
    public class ArquivosController : BaseController
    {
        private SiteDB db = new SiteDB();

        // GET: Arquivos        
        public ActionResult Index()
        {
            var model = db.Arquivos.OrderByDescending(r => r.Data);

            if (Request.IsAjaxRequest())
            {
                return PartialView("_Arquivos", model);
            }

            return View(model);
        }

        // GET: Arquivos/Create        
        public ActionResult Create()
        {
            return View();
        }

        // POST: Arquivos/Create        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Data,Nome,Tag,Texto")] Arquivo arquivo)
        {
            if (ModelState.IsValid)
            {
                db.Arquivos.Add(arquivo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(arquivo);
        }

        // GET: Arquivos/Edit/5        
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Arquivo arquivo = db.Arquivos.Find(id);
            if (arquivo == null)
            {
                return HttpNotFound();
            }
            return View(arquivo);
        }

        // POST: Arquivos/Edit/5        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Data,Nome,Tag, Texto")] Arquivo arquivo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(arquivo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(arquivo);
        }

        // GET: Arquivos/Delete/5        
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Arquivo arquivo = db.Arquivos.Find(id);
            if (arquivo == null)
            {
                return HttpNotFound();
            }
            return View(arquivo);
        }

        // GET: Arquivos/DeleteAll        
        public ActionResult DeleteAll()
        {
            var model = db.Arquivos;

            if (model.Count() == 0)
            {
                TempData["Message"] = "Não existem arquivos para serem excluídos.";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // POST: Arquivos/Delete/5        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Arquivo arquivo = db.Arquivos.Find(id);
            db.Arquivos.Remove(arquivo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Arquivos/Delete/5        
        [HttpPost, ActionName("DeleteAll")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed()
        {
            var rows = db.Arquivos;

            foreach (var row in rows)
            {
                db.Arquivos.Remove(row);
            }
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Upload()
        {
            return View();
        }

        public ActionResult SalvaArquivosUpload()
        {
            foreach (var fileName in Request.Files.AllKeys)
            {
                var file = Request.Files[fileName];

                if (file != null && file.ContentLength > 0)
                {
                    var fName    = Path.GetFileName(file.FileName);
                    var Conteudo = Utilidades.ReadData(file);

                    var Nome = "";
                    var Tag  = "";
                    Conteudo = Utilidades.RetornaValorTag(Conteudo, "<nome>", out Nome);
                    Conteudo = Utilidades.RetornaValorTag(Conteudo, "<tag>" , out Tag);
                    
                    if (Nome.Equals(""))
                        Nome = Path.GetFileNameWithoutExtension(file.FileName);


                    Arquivo arquivo = new Arquivo(Nome, Tag, Conteudo);
                    db.Arquivos.Add(arquivo);
                }
            }
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Treinamento()
        {
            var model = db.Arquivos;
            
            if (model.Count() <= 1)
            {
                TempData["Message"] = "É necessário existir no mínimo um arquivo para realizar o treinamento.";
                return RedirectToAction("Index");
            }
            
            ExportaArquivosServidor();

            RealizaTreinamentoWeka();

            return View(model);
        }

        private void ExportaArquivosServidor()
        {
            // Esvazia o diretório de dados
            Utilidades.DeleteArquivosDiretorio(DiretorioDadosServidor);

            var model = db.Arquivos.Where(a => !a.Tag.Equals(""));

            foreach (var arquivo in model)
            {
                arquivo.GerarArquivoFisico(DiretorioDadosServidor);
            }
        }

        private Instances ImportaArquivosServidor ()
        {
            // Cria as instancias dos dados de treinamento, exportados para a pasta /Dados/
            TextDirectoryLoader loader = new TextDirectoryLoader();
            loader.setDirectory(new java.io.File(DiretorioDadosServidor));
            loader.setCharSet("UTF-8");
            return loader.getDataSet();
        }

        private void RealizaTreinamentoWeka()
        {
            TempData["DataTreinamento"] = DateTime.Now;

            Instances dadosTreinamento = ImportaArquivosServidor();

            // Aplica o filter StringToWordVector
            weka.filters.Filter[] filters = new weka.filters.Filter[2];
            filters[0] = new StringToWordVector();
            filters[1] = AttributeSelectionFilter(2);
            
            weka.filters.MultiFilter filter = new weka.filters.MultiFilter();
            filter.setInputFormat(dadosTreinamento);
            filter.setFilters(filters);
            
            // Cria o Classificador a partir dos dados de treinamento
            FilteredClassifier classifier = new FilteredClassifier();
            classifier.setFilter(filter);
            classifier.setClassifier(new NaiveBayes());
            classifier.buildClassifier(dadosTreinamento);

            // Realiza um CrossValidation pra expôr as estatísticas do Classificador
            /*
            Evaluation eval = new Evaluation(dadosTreinamento);
            eval.crossValidateModel(classifier, dadosTreinamento, 10, new java.util.Random(1));

            var EstatisticasTexto = 
                "Instâncias Corretas: \t" + eval.correct() + "  (" + Math.Round(eval.pctCorrect(),2) + "%)" + System.Environment.NewLine
              + "Instâncias Incorretas: \t" + eval.incorrect() + "  (" + Math.Round(eval.pctIncorrect(),2) + "%)" + System.Environment.NewLine
              + "Total de Instâncias: \t\t" + eval.numInstances();


            TempData["TreinamentoRealizado"] = EstatisticasTexto;
            */

            // Salva o Classificador (model) como /Classificador/Classificador.model
            SerializationHelper.write(string.Format("{0}Classificador.model", DiretorioClassificadorServidor), classifier);

            // Salva os dados de treinamento como /Classificador/DadosTreinamento.arff
            Utilidades.SalvarArff(dadosTreinamento, DiretorioClassificadorServidor, "DadosTreinamento.arff");
        }

        private AttributeSelection AttributeSelectionFilter(int opcao)
        {
            AttributeSelection filter = new AttributeSelection();

            // CfSubsetEval e BestFirst
            if (opcao == 1)
            {
                weka.attributeSelection.CfsSubsetEval evaluator = new weka.attributeSelection.CfsSubsetEval();
                filter.setEvaluator(evaluator);

                weka.attributeSelection.BestFirst search = new weka.attributeSelection.BestFirst();
                filter.setSearch(search);

            }
            // InfoGainAttributeEval e Ranker
            else if (opcao == 2)
            {
                // Evaluator
                weka.attributeSelection.InfoGainAttributeEval evaluator = new weka.attributeSelection.InfoGainAttributeEval();
                //evaluator.setMissingSeparate(true);
                filter.setEvaluator(evaluator);

                // Search strategy: best first (default values)
                weka.attributeSelection.Ranker search = new weka.attributeSelection.Ranker();
                search.setThreshold(0);
                filter.setSearch(search);
            }

            return filter;
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}