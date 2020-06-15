using DevelopersChallenge2.Models;
using DevelopersChallenge2.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DevelopersChallenge2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string message, bool? error)
        {
            var transactionService = new TransactionService();
            var transactions = transactionService
                .GetAllActives()
                .OrderBy(x => x.TransactionDate)
                .ToList()
                .Select(x => new TransactionViewModel
                {
                    Bank = x.Bank.Name,
                    Description = x.Description,
                    TransactionDate = x.TransactionDate,
                    TransactionType = x.TransactionType,
                    Value = x.Value < 0
                        ? "<span style='color:red'>" + x.Value.ToString("C") + "</span>"
                        : "<span style='color:blue'>" + x.Value.ToString("C") + "</span>"
                })
                .ToList();

            var homeViewModel = new HomeViewModel
            {
                Message = message,
                Error = error,
                DatagridData = transactions
            };

            return View(homeViewModel);
        }

        [HttpPost]
        public ActionResult Upload()
        {
            try
            {
                if (Request.Files.Count > 0 && Request.Files[0].ContentLength > 0)
                {
                    if (!System.IO.Directory.Exists(Server.MapPath("~/Temp")))
                    {
                        System.IO.Directory.CreateDirectory(Server.MapPath("~/Temp"));
                    }

                    var file = Request.Files[0];
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Temp"), fileName);
                    file.SaveAs(path);

                    var fileContent = System.IO.File.ReadAllLines(path);
                    System.IO.File.Delete(path);

                    var ofxSource = new StringBuilder();

                    foreach (var line in fileContent)
                    {
                        if (line.Trim().Length == 0)
                            continue;

                        if (line[0] != '<')
                        {
                            var keyPair = line.Split(':');
                            switch (keyPair[0])
                            {
                                case "DATA":
                                    if (keyPair[1] != "OFXSGML")
                                    {
                                        throw new Exception("Formato do arquivo OFX inválido. Esperado o formato OFXSGML.");
                                    };
                                    break;
                                case "VERSION":
                                    if (keyPair[1] != "102")
                                    {
                                        throw new Exception("Versão do arquivo não suportada. Esperado a versão 1.0.2.");
                                    };
                                    break;
                                case "COMPRESSION":
                                    if (keyPair[1] != "NONE")
                                    {
                                        throw new Exception("O arquivo OFX não deve estar comprimido.");
                                    };
                                    break;
                            }
                        }
                        else
                        {
                            ofxSource.AppendLine(line);
                        }
                    }

                    var transactionService = new TransactionService();
                    transactionService.ProcessOfx(ofxSource.ToString());
                }
                else
                {
                    throw new Exception("Selecione um arquivo OFX para conciliação.");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", new { message = ex.Message, error = true });
            }

            return RedirectToAction("Index", new { message = "A importação foi realizada com sucesso.", error = false });
        }
    }
}