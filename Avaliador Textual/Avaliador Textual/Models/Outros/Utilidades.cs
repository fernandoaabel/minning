using System.Web;
using System.IO;
using System.Text;
using System.Net;
using System;
using System.Text.RegularExpressions;
using weka.core.converters;
using weka.core;

namespace Avaliador_Textual.Models.Outros
{
    public class Utilidades
    {
        private static readonly Regex _tags_ = new Regex(@"<[^>]+?>", RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex _notOkCharacter_ = new Regex(@"[^\w;&#@.:/\\?=|%!() -]", RegexOptions.Compiled);

        public static bool CheckURLValid(string source)
        {
            Uri uriResult;
            bool result = Uri.TryCreate(source, UriKind.Absolute, out uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            return result;
        }

        public static string BuscaHTML(string url)
        {
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            var pagina = wc.DownloadString(url);
            return WebUtility.HtmlDecode(pagina);
        }

        private static string RemoveTag(string html, string startTag, string endTag)
        {
            Boolean bAgain;
            do
            {
                bAgain = false;
                int startTagPos = html.IndexOf(startTag, 0, StringComparison.CurrentCultureIgnoreCase);
                if (startTagPos < 0)
                    continue;
                int endTagPos = html.IndexOf(endTag, startTagPos + 1, StringComparison.CurrentCultureIgnoreCase);
                if (endTagPos <= startTagPos)
                    continue;
                html = html.Remove(startTagPos, endTagPos - startTagPos + endTag.Length);
                bAgain = true;
            } while (bAgain);
            return html;
        }

        private static string SingleSpacedTrim(string inString)
        {
            StringBuilder sb = new StringBuilder();
            Boolean inBlanks = false;
            foreach (Char c in inString)
            {
                switch (c)
                {
                    case '\r':
                    case '\n':
                    case '\t':
                    case ' ':
                        if (!inBlanks)
                        {
                            inBlanks = true;
                            sb.Append(' ');
                        }
                        continue;
                    default:
                        inBlanks = false;
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString().Trim();
        }

        public static string LimpaHTML(string html)
        {
            html = HttpUtility.UrlDecode(html);
            html = HttpUtility.HtmlDecode(html);

            html = RemoveTag(html, "<!--", "-->");
            html = RemoveTag(html, "<script", "</script>");
            html = RemoveTag(html, "<style", "</style>");

            //replace matches of these regexes with space
            html = _tags_.Replace(html, " ");
            html = _notOkCharacter_.Replace(html, " ");
            html = SingleSpacedTrim(html);

            return html;
        }

        public static string ReadData(HttpPostedFileBase file)
        {
            // Read bytes from http input stream
            BinaryReader b = new BinaryReader(file.InputStream);
            byte[] binData = b.ReadBytes(file.ContentLength);

            string result = Encoding.UTF8.GetString(binData);

            return result;
        }
        
        public static string RetornaValorTag(string conteudo, string tag, out string valor)
        {
            valor = "";
            var tagFinal = tag.Insert(1, "/");

            // Verifica se contém a tag
            if (conteudo.ToLower().Contains(tag))
            {
                var indexInicial = conteudo.IndexOf(tag);
                var indexFinal = conteudo.IndexOf(tagFinal, indexInicial) - indexInicial - tag.Length;

                // Busca valor da tag e remove a linha do texto
                valor = conteudo.Substring(indexInicial + tag.Length, indexFinal);
                conteudo = conteudo.Remove(indexInicial, tag.Length + valor.Length + tagFinal.Length);
            }

            return conteudo;
        }

        public static void SalvarArff(Instances Data, string Diretorio, string NomeArquivo)
        {
            ArffSaver saver = new ArffSaver();
            saver.setInstances(Data);
            saver.setFile(new java.io.File(string.Format("{0}" + NomeArquivo, Diretorio)));
            saver.writeBatch();
        }

        public static void DeleteArquivosDiretorio (string path)
        {
            // Apagar sub-pastas e ficheiros?
            var subfolders = Directory.GetDirectories(path);
            foreach (var s in subfolders)
                Directory.Delete(s, true);
        }
    }
}