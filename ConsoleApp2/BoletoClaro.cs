using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using System.Drawing;
using System.Windows;
using System.Text;
using OpenQA.Selenium.Support.UI;
using System.Threading;


namespace ConsoleApp2
{
    class BoletoClaro
    {
        private const string IE_DRIVER_PATH = @"c:\web";
        static void Main(string[] args)
        {
            var driver = new InternetExplorerDriver(IE_DRIVER_PATH);

            Console.WriteLine("Qual nome da base?");
            String robo_base = Console.ReadLine();


            Console.WriteLine("Qual nome do arquivo de saida?");
            String robo_saida = Console.ReadLine();


            Console.Clear();


            string[] linhas =  File.ReadAllLines("\\\\172.21.0.6\\winover$\\PLANEJAMENTO\\55.CLARO FIXO\\04.Base Robo\\"+ robo_base +".csv");

            ConsoleSpinner spinner = new ConsoleSpinner();
            spinner.Delay = 300;

            


            foreach (var linha in linhas)
            {

                driver.Url = "http://clarofixo.claro.com.br/conta_detalhe/";

                var dadosUsuario = linha.Split(';');
                var cpf = dadosUsuario[5];
                var conta = dadosUsuario[0].Trim();
                var localidade = dadosUsuario[4];
                var dd = dadosUsuario[2].Substring(0, 2);
                var mm = dadosUsuario[2].Substring(3, 2);
                var yy = dadosUsuario[2].Substring(6, 4);
                var atraso = dadosUsuario[6];
                var telefone = dadosUsuario[1];
                var nome = dadosUsuario[3];
                var data = DateTime.Now.ToShortDateString();

                

                Thread.Sleep(2000);
                Boolean Closed;
                Console.WriteLine("Lendo dados do cliente");
                TextWriter tw = new StreamWriter("\\\\172.21.0.6\\winover$\\PLANEJAMENTO\\55.CLARO FIXO\\05.Base Robo Tratado\\" + robo_saida +".txt", true);

                if (cpf.Length <= 11)
                {
                    if(cpf.Length < 11)
                    {
                        while(cpf.Length < 11)
                        {
                            Console.WriteLine("Adicionando Zeros ao CPF");
                            cpf = cpf.PadLeft(11, '0');
                        }
                    }

                    try
                    {
                        driver.FindElement(By.Name("txtCPF")).SendKeys(cpf);
                        driver.FindElement(By.Name("codigoClienteInformado")).SendKeys(conta);
                        driver.FindElement(By.Name("diaInformado")).SendKeys(dd);
                        driver.FindElement(By.Name("mesInformado")).SendKeys(mm);
                        driver.FindElement(By.Name("anoInformado")).SendKeys(yy);

                        String localidadeEsperada = "SP";

                        if (localidade.Equals(localidadeEsperada))
                        {
                            driver.FindElement(By.Name("ufInformado")).SendKeys("Estado de São Paulo");
                        } else
                        {
                            driver.FindElement(By.Name("ufInformado")).SendKeys("Demais Estados");
                        }

                        driver.FindElement(By.Id("submit")).Click();

                        String teste2 = driver.PageSource;
                        String Falha = driver.FindElement(By.TagName("b")).Text;
                        String Alert = "Fatura não encontrada";

                        if (Falha.Equals(Alert))
                        {
                            Console.WriteLine("                          Não foi encontrado boleto                              ");
                            Thread.Sleep(3000);
                            tw.WriteLine(conta + "," + telefone + "," + dadosUsuario[2] + "," + nome + "," + localidade + "," + cpf + "," + atraso + "," + "Sem Fatura" + "," + data);
                            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                            Console.Clear();
                            Closed = false;
                        }
                        else
                        {
                            Console.WriteLine("                            Cadastrando Boleto                                   ");
                            Thread.Sleep(3000);
                            String boleto = driver.FindElement(By.TagName("code")).Text;
                            tw.WriteLine(conta + "," + telefone + "," + dadosUsuario[2] + "," + nome + "," + localidade + "," + cpf + "," + atraso + "," + boleto);
                            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
                            Console.Clear();
                            Closed = false;
                        }
                        if(Closed == false)
                        {
                            tw.Close();
                        }
                        
                    }
                    catch (Exception e)
                    {
                        Console.Write(e);
                    }
                }
                tw.Close();
            }
            Console.WriteLine("Operação Concluida");
        }
    }
}
