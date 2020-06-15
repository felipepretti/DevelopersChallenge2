using DevelopersChallenge2.DAL.Context;
using DevelopersChallenge2.Model;
using DevelopersChallenge2.Model.Enums;
using DevelopersChallenge2.Repository.Interfaces;
using DevelopersChallenge2.Repository.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace DevelopersChallenge2.Service
{
    public class TransactionService : GenericoService<TransactionRepository, SqlDbContext>
    {
        public TransactionService() : this(null) { }
        public TransactionService(IRepositoryFactory repositoryFactory) : base(repositoryFactory) { }

        public IQueryable<Transaction> GetAllActives()
        {
            return Repository.GetAllActives();
        }

        public void ProcessOfx(string ofxSource)
        {
            if (string.IsNullOrEmpty(ofxSource))
                throw new Exception("O conteúdo do arquivo OFX é inválido.");

            using (var scope = new System.Transactions.TransactionScope())
            {
                var transactions = new List<Transaction>();

                var xml = GenerateXmlFromOfx(ofxSource);
                var bankCode = xml.FirstChild["BANKMSGSRSV1"].FirstChild["STMTRS"]["BANKACCTFROM"]["BANKID"].InnerText;
                var bankTranList = xml.FirstChild["BANKMSGSRSV1"].FirstChild["STMTRS"]["BANKTRANLIST"].ChildNodes;

                var bankService = new BankService(RepositoryFactory);
                var bank = bankService.GetByCode(bankCode);

                if (bank == null)
                    throw new Exception(string.Format("Necessário cadastrar o banco \"{0}\" antes de importar as transações.", bankCode));

                foreach (XmlNode node in bankTranList)
                {
                    if (node.Name != "STMTTRN")
                        continue;

                    var offSet = Convert.ToInt32(node["DTPOSTED"].InnerText.Substring(15).Split(':')[0]);

                    var utcDate = new DateTimeOffset(Convert.ToInt32(node["DTPOSTED"].InnerText.Substring(0, 4)),
                                                     Convert.ToInt32(node["DTPOSTED"].InnerText.Substring(4, 2)),
                                                     Convert.ToInt32(node["DTPOSTED"].InnerText.Substring(6, 2)),
                                                     Convert.ToInt32(node["DTPOSTED"].InnerText.Substring(8, 2)),
                                                     Convert.ToInt32(node["DTPOSTED"].InnerText.Substring(10, 2)),
                                                     Convert.ToInt32(node["DTPOSTED"].InnerText.Substring(12, 2)),
                                                     new TimeSpan(offSet, 0, 0))
                        .UtcDateTime;

                    var transaction = new Transaction
                    {
                        IdBank = bank.Id,
                        Description = node["MEMO"].InnerText.Trim(),
                        TransactionType = (TransactionType)Enum.Parse(typeof(TransactionType), node["TRNTYPE"].InnerText, true),
                        Value = Convert.ToDecimal(node["TRNAMT"].InnerText.Replace('.', ',')),
                        TransactionDate = utcDate
                    };

                    using (var sha256Hash = SHA256.Create())
                    {
                        transaction.Hash = GetHash(sha256Hash, transaction.TransactionType.ToString()
                                    + transaction.TransactionDate.ToString()
                                    + transaction.Value.ToString()
                                    + transaction.Description.ToUpper().Trim());
                    }

                    transactions.Add(transaction);
                }

                var startDate = transactions
                    .OrderBy(x => x.TransactionDate)
                    .Select(x => x.TransactionDate)
                    .FirstOrDefault();
                var endDate = transactions
                    .OrderByDescending(x => x.TransactionDate)
                    .Select(x => x.TransactionDate)
                    .FirstOrDefault();

                var transactionsExists = Repository.GetAllActives()
                    .Where(x => x.TransactionDate >= startDate
                             && x.TransactionDate <= endDate)
                    .Select(x => x.Hash)
                    .ToList();

                var transactionsPersist = transactions
                    .Where(x => !transactionsExists.Contains(x.Hash));

                Repository.InsertBatch(transactionsPersist);

                scope.Complete();
            }
        }

        private XmlDocument GenerateXmlFromOfx(string ofxSource)
        {
            try
            {
                var matches = Regex.Matches(ofxSource, "\\<\\w+\\>([\\w \\[\\]:\\-\\+.!?\\/]+)");

                for (int i = 0; i < matches.Count; i++)
                {
                    var tagName = Regex.Match(matches[i].Value, "(?<=\\<)(.*?)(?=\\>)");
                    var newTag = matches[i].Value + "</" + tagName + ">";

                    if (ofxSource.IndexOf(newTag) == -1)
                    {
                        ofxSource = ofxSource.Replace(matches[i].Value, newTag);
                    }
                }

                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(ofxSource);

                return xmlDoc;
            }
            catch
            {
                throw new Exception("Não foi possível realizar a leitura do arquivo.");
            }
        }

        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }
    }
}
