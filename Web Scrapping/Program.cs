using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace Web_Scrapping
{
    class Program
    {
        static void Main(string[] args)
        {
            string verficar = Console.ReadLine();
            string resultado = VerificarCnpjOuCpf(verficar);

            Console.WriteLine(resultado);
        }

        public static string VerificarCnpjOuCpf(string cnpj)
        {
            string verify = "";
            using (IWebDriver driver = new ChromeDriver())
            {
                driver.Url = "https://www.situacao-cadastral.com/";

                IWebElement text = driver.FindElement(By.Id("doc"));

                string valor = cnpj;
                char[] count = valor.ToCharArray();
                int cot = 0;
                char[] nums = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
                for (int i = 0; i < valor.Length; i++)
                {
                    for (int j = 0; j < nums.Length; j++)
                    {
                        if (count[i] == nums[j])
                        {
                            cot += 1;
                        }
                    }
                }

                text.SendKeys(valor + Keys.Enter);

                Thread.Sleep(2200);

                if (IsElementPresent(By.Id("mesangem"), driver))
                {
                    if (driver.FindElement(By.Id("mesangem")).Text == "Tente novamente!")
                    {
                        text.Submit();
                    }
                }

                if (IsElementPresent(By.ClassName("vrd"), driver))
                {
                    IWebElement ativo = driver.FindElement(By.ClassName("vrd"));
                    if (cot > 12)
                    {
                        IWebElement empresa = driver.FindElement(By.CssSelector(".nome"));
                        IWebElement localidade = driver.FindElement(By.ClassName("cidade"));
                        IWebElement pendencias = driver.FindElement(By.CssSelector(".texto p"));

                        verify = $"O CNPJ consultado a { ativo.Text} nome da empresa {empresa.Text}, Matriz:{localidade.Text}. Pendências {pendencias.Text}";
                    }
                    else
                    {
                        IWebElement Nome = driver.FindElement(By.CssSelector(".nome"));
                        IWebElement pendencias = driver.FindElement(By.CssSelector(".texto"));
                        verify = $"O CPF consultado a { ativo.Text} do nome {Nome.Text}. Pendências{pendencias.Text}";
                    }
                }
                else if (IsElementPresent(By.Id("mensagem"), driver))
                {
                    IWebElement ativo = driver.FindElement(By.Id("mensagem"));
                    verify = (ativo.Text == "CNPJ inválido") ? $"O {ativo.Text}" : $"O {ativo.Text}";
                }
                else if (IsElementPresent(By.ClassName("vrm"), driver))
                {
                    IWebElement ativo = driver.FindElement(By.ClassName("vrm"));
                    if (cot > 11)
                    {
                        if (ativo.Text == "Situação: Inexistente")
                        {
                            IWebElement federal = driver.FindElement(By.CssSelector(".texto"));
                            verify = $"O CNPJ consultado a { ativo.Text}. {federal.Text} ";
                        }
                        else
                        {
                            IWebElement empresa = driver.FindElement(By.CssSelector(".nome"));
                            IWebElement pendencias = driver.FindElement(By.CssSelector(".texto p"));
                            verify = $"O CNPJ consultado a { ativo.Text} nome da empresa {empresa.Text}. Motivo de encerramento {pendencias.Text}";
                        }
                    }
                    else
                    {
                        if (ativo.Text == "Situação: Inexistente")
                        {
                            IWebElement Federa = driver.FindElement(By.CssSelector(".texto p"));
                            verify = $"O CPF consultado a { ativo.Text}. {Federa.Text}";
                        }
                        else
                        {
                            IWebElement Nome = driver.FindElement(By.CssSelector(".nome"));
                            IWebElement pendencias = driver.FindElement(By.CssSelector(".texto"));
                            verify = $"O CPF consultado a { ativo.Text} do nome {Nome.Text}. Pendências{pendencias.Text}";
                        }
                    }
                }
                else if (IsElementPresent(By.ClassName("amr"), driver))
                {
                    IWebElement ativo = driver.FindElement(By.ClassName("amr"));
                    verify = (cot > 11) ? $"O CNPJ consultado a {ativo.Text}" : $"O CNPJ consultado a {ativo.Text}";
                }
                else
                {
                    verify = "CPF ou CNPJ não foi encontrado!!!";
                }
                driver.Close();
                driver.Quit();
            }
            return verify;
        }
        private static bool IsElementPresent(By by, IWebDriver driver)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}
