using Avaliador_Textual.Models;
using System;
using System.Web.Mvc;

namespace Avaliador_Textual.Controllers
{
    public abstract class BaseController : Controller
    {

        #region Properties

        protected string DiretorioDadosServidor { get { return string.Format("{0}Dados\\", Server.MapPath(@"\")); } }
        protected string DiretorioClassificadorServidor { get { return string.Format("{0}Classificador\\", Server.MapPath(@"\")); } }
        protected string DiretorioTestesServidor { get { return string.Format("{0}Testes\\", Server.MapPath(@"\")); } }
        
        #endregion

        #region Exceptions

        public class ClassificationException : Exception
        {
            public ClassificationException(string message) : base(message)
            {
            }
        }

        #endregion
    }
}