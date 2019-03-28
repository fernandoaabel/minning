using Avaliador_Textual.Models.Outros;
using silabas;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Text.RegularExpressions;
using ptstemmer;
using Newtonsoft.Json;

namespace Avaliador_Textual.Models
{
    public class Arquivo
    {        
        #region Construtores

        public Arquivo()
        {
            // This is required for EF
        }

        public Arquivo(string nome, string tag, string texto)
        {
            Nome  = nome;
            Tag   = tag;
            Texto = texto;
        }

        public Arquivo(string texto, string url)
        {
            if (!texto.Equals(string.Empty))
            {
                Nome = texto.Substring(0, Math.Min(texto.Length, 30));
                Texto = texto;
            }
            else if (!url.Equals(string.Empty))
            {
                var html      = Utilidades.BuscaHTML(url);
                var textoHtml = Utilidades.LimpaHTML(html);

                Nome = url;
                Texto = textoHtml;
            }
        }

        public void InicializaArquivo()
        {
            Data = DateTime.Now;
            if (!string.IsNullOrWhiteSpace(Tag))
                Tag = Tag.Trim();

            if (!Texto.Equals(string.Empty))
            {
                // Realiza o PreProcessamento para a avaliação da qualidade
                PreProcessamento();

                // Realiza a contagem de palavras únicas no texto, utilizado para o WordCloud
                CriaFrequenciaPalavras();

                // Realiza a avaliação da apreensibilidade
                Apreensibilidade();
            }
        }

        #endregion

        #region Atributos

        public int Id { get; set; }

        [Required]
        [Display(Name ="Alterado")]
        public DateTime Data { get; set; }

        [Required]
        [Display(Name = "Nome/Site")]
        public string Nome { get; set; }
        
        [Display(Name = "Tag")]
        public string Tag { get; set; }

        [Required]
        [Display(Name = "Texto")]
        [DataType(DataType.MultilineText)]
        public string Texto { get; set; }

        [Display(Name = "Texto Formatado")]
        [DataType(DataType.MultilineText)]
        public string TextoFormatado { get; set; }

        [Display(Name = "Número de Palavras do Texto")]
        public int NroPalavras { get; set; }

        [Display(Name = "Número de Frases do Texto")]
        public int NroFrases { get; set; }

        [Display(Name = "Número de Sílabas do Texto")]
        public int NroSilabas { get; set; }

        [Display(Name = "Fernàndez-Huerta")]
        public double IndiceApreensibilidade { get; set; }

        [NotMapped]
        public List<Token> _listaPalavras { get; set; }

        [Display(Name = "Classificação")]
        public string Classificacao { get; set; }

        [Display(Name = "Frequencia das Palavras")]
        public string FrequenciaPalavras { get; set; }

        #endregion

        #region Apreensibilidade

        public void Apreensibilidade()
        {
            NroPalavras = ContagemPalavras(Texto);
            NroFrases   = ContagemFrases(Texto);
            NroSilabas  = ContagemSilabas(Texto);

            //IndiceApreensibilidade = CalculaFleschReadingEase();
            IndiceApreensibilidade = CalculaFernandezHuerta();
        }

        public int ContagemPalavras(string texto)
        {
            int cont = 0;

            // Desconsidera espaços antes e depois dos textos
            texto = texto.Trim();

            // Loop sobre todos os caracteres do texto
            for (int i = 1; i < texto.Length; i++)
            {
                // Se o caracter da posição anterior é um espaço em branco ou quebra de linha
                if (char.IsWhiteSpace(texto[i - 1]) == true)
                {
                    // Se o caracter da posição atual é uma letra ou número
                    if (char.IsLetterOrDigit(texto[i]) == true)
                    {
                        cont++;
                    }
                }
            }
            // Conta a primeira palavra
            if (texto.Length > 1)
                cont++;

            // Retorna o número de palavras
            return cont;
        }

        public int ContagemFrases(string texto)
        {
            // Caracteres que indicam terminação de frases
            char[] separatingChars = { '.', '?', '!', ':', ';', '-', '\0' };
            // Divide o texto em frases
            string[] frases = texto.Split(separatingChars, StringSplitOptions.RemoveEmptyEntries);
            // Retorna o número de frases
            return frases.Length;
        }

        public int ContagemSilabas(string texto)
        {
            // Utilização da JAVA API de Oliveira (2007)
            var s = SilabasPT.separa(texto);
            // Retorna o número de sílabas do texto
            return s.size();
        }

        public double CalculaFleschReadingEase()
        {
            string texto = Texto;

            // Realiza a contagem de palavras, frases e sílabas do texto
            var nroPalavras = ContagemPalavras(texto);
            var nroFrases   = ContagemFrases(texto);
            var nroSilabas  = ContagemSilabas(texto);
            
            // Variável Contagem de Palavras / Variável Contagem de Frases
            var ASL     = (float)nroPalavras / nroFrases;
            // Variável Contagem de Sílabas / Variável Contagem de Palavras
            var ASW     = (float)nroSilabas / nroPalavras;
            
            // Fórmula de Flesch Reading Ease
            var resultado = 206.835 - (1.015 * ASL) - (84.6 * ASW);

            resultado = Math.Max(resultado, 0);
            resultado = Math.Min(resultado, 100);

            return resultado;
        }

        public double CalculaFernandezHuerta()
        {
            string texto = Texto;

            // Realiza a contagem de palavras, frases e sílabas do texto
            var nroPalavras = ContagemPalavras(texto);
            var nroFrases   = ContagemFrases(texto);
            var nroSilabas  = ContagemSilabas(texto);

            // Variável Contagem de Palavras / Variável Contagem de Frases
            var ASL = (float)nroPalavras / nroFrases;
            // Variável Contagem de Sílabas / Variável Contagem de Palavras
            var ASW = (float)nroSilabas / nroPalavras;

            // Fórmula de Fernandez-Huerta
            var resultado = 206.84 - (1.02 * ASL) - (60 * ASW);

            // Limita o resultado a uma escala de 0 a 100
            resultado = Math.Max(resultado, 0);
            resultado = Math.Min(resultado, 100);

            return Math.Round(resultado);
        }
        
        #endregion

        #region Formatação do Texto

        private void PreProcessamento()
        {
            // Converte o texto para minúsculas
            TextoFormatado = Texto.ToLower();

            // Remove os caracteres especiais e números
            TextoFormatado = RemoverCaracteresEspeciais(TextoFormatado);

            // Substitui os caracteres com acentuação pelo respectivo caractere sem acentuação
            TextoFormatado = RemoverAcentos(TextoFormatado);
            
            // Tokenization, transforma o Texto em uma Lista de Palavras
            _listaPalavras = Tokenization(TextoFormatado);

            // Remove as Stopwords da Lista de Palavras
            RemoverStopwords(MyStopwordsHandler.linguaPortuguesa, _listaPalavras);

            //Converte as palavras nos seus radicais, através da técnica de Stemming de PTStemmer
            Stemming(_listaPalavras);

            // Recompõe o Texto a partir da lista de palavras contendo os radicais
            TextoFormatado = string.Empty;
            foreach (var palavra in _listaPalavras)
                TextoFormatado = TextoFormatado + " " + palavra.Radical;

        }

        private string[] Delimitadores()
        {
            string[] lista = {" ","'","´","`","*",",",";",",",".","\n","\r","\t","}","{","-","_","|","=","[","]","&","(",")","\"","/",":"};

            return lista;
        }

        public string RemoverAcentos(string texto)
        {
            string comAcentos = "ÄÅÁÂÀÃäáâàãÉÊËÈéêëèÍÎÏÌíîïìÖÓÔÒÕöóôòõÜÚÛüúûùÇç";
            string semAcentos = "AAAAAAaaaaaEEEEeeeeIIIIiiiiOOOOOoooooUUUuuuuCc";

            for (int i = 0; i < comAcentos.Length; i++)
            {
                texto = texto.Replace(comAcentos[i].ToString(), semAcentos[i].ToString());
            }
            return texto;
        }

        public string RemoverCaracteresEspeciais(string texto)
        {
            string pattern = @"[^a-záéíóúàèìòùâêîôûãõç\s]";
            Regex rgx = new Regex(pattern);
            string replacement = "";

            return rgx.Replace(texto, replacement);
        }

        public List<Token> Tokenization(string texto)
        {
            var lista = new List<Token>();
            string[] textoArray = texto.Split(Delimitadores(), StringSplitOptions.RemoveEmptyEntries);

            // Percorre palavra por palavra no texto
            foreach (string palavra in textoArray)
            {
                int num;
                bool numerico = int.TryParse(palavra, out num);
                if (!numerico)
                {
                    lista.Add(new Token(palavra));
                }
            }
            return lista;
        }

        private void RemoverStopwords (int lingua, List<Token> lista)
        {
            //Remove as palavras da lista conforme a lista de stopwords
            MyStopwordsHandler stopword = new MyStopwordsHandler(lingua);
            stopword.RemoverStopWords(lista);
        }

        private void Stemming(List<Token> lista)
        {
            var stemmer = Stemmer.StemmerFactory(Stemmer.StemmerType.ORENGO);
            foreach(Token tk in lista)
            {
                tk.Radical = stemmer.wordStemming(tk.Palavra);
            }
        }

        #endregion

        #region Outros

        /* CriaFrequenciaPalavras
         * Método responsável por construir uma lista de palavras únicas, e suas frequências no texto
         * As Stopwords são removidas da lista e um objeto JSON da lista é formado.
         */
        public void CriaFrequenciaPalavras()
        {
            var textoLower = RemoverCaracteresEspeciais(Texto.ToLower());
            string[] textoArray = textoLower.Split(Delimitadores(), StringSplitOptions.RemoveEmptyEntries);

            var lista = new List<Token>();

            // Percorre palavra por palavra no texto
            foreach (string palavra in textoArray)
            {
                Token token = lista.Find(t => t.Palavra == palavra);

                if (token != null)
                    token.Frequencia++;
                else
                {
                    int num;
                    bool numerico = int.TryParse(palavra, out num);
                    if (!numerico)
                    {
                        lista.Add(new Token(palavra, 1));
                    }
                }
            }

            RemoverStopwords(MyStopwordsHandler.linguaPortuguesa, lista);

            var json = JsonConvert.SerializeObject(lista);
            FrequenciaPalavras = json.ToString();
        }

        /* GerarArquivoFisico
         * Método responsável por criar o arquivo físico no diretório do Servidor em "diretorioDadosServidor"
         */
        public void GerarArquivoFisico(string diretorioDadosServidor)
        {
            // Gera o arquivo físico no server
            var subpasta = diretorioDadosServidor;

            if (!Tag.Equals(""))
            {
                subpasta = Path.Combine(diretorioDadosServidor, Tag);
                if (!File.Exists(subpasta))
                    Directory.CreateDirectory(subpasta);
            }

            // Create a file name for the file you want to create. 
            subpasta = Path.Combine(subpasta, Path.GetRandomFileName());

            if (!File.Exists(subpasta))
            {
                using (FileStream fs = File.Create(subpasta))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Write(TextoFormatado);
                    }
                }
            }
        }
       
        #endregion
    }
}