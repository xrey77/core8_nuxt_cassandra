using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using Cassandra;
using Cassandra.Mapping;
using Cassandra.Data.Linq;
using core8_nuxt_cassandra.Entities;
using core8_nuxt_cassandra.Helpers;
using core8_nuxt_cassandra.Models;

namespace core8_nuxt_cassandra.Services
{
    public interface IProductService {
        IEnumerable<Product> ListAll(int pg);
        Task<IEnumerable<Product>> SearchAll(int page, string key);
        Task<int> TotPageSearch(int pg, string key);
        Task<int> TotPage();
        Task CreateProduct(Product prod);
        void ProductUpdate(Product prod);
        Task<bool> ProductDelete(Guid id);
        void UpdateProdPicture(Guid id, string file);
        Product GetProductById(Guid id);
    }

    public class ProductService : IProductService
    {
        private readonly Cassandra.ISession session;   

        public ProductService(IConfiguration config)        
        {        
            var cluster = Cluster.Builder()
                .AddContactPoint(config["CassandraSettings:ContactPoints"])
                .WithPort(int.Parse(config["CassandraSettings:port"]))
                .WithCredentials(config["CassandraSettings:Username"], config["CassandraSettings:Password"])
                .WithExecutionProfiles(options => options
                    .WithProfile("profile1", profileBuilder => profileBuilder
                        .WithReadTimeoutMillis(10000)
                        .WithConsistencyLevel(ConsistencyLevel.LocalQuorum)))
                .Build();

            session = cluster.Connect(config["CassandraSettings:Keyspace"]);
        }        

        public async Task<int> TotPage() {
            var perpage = 5;
            var totrecs = await session.ExecuteAsync(new SimpleStatement("SELECT count(*) FROM core8.products"));
            var row = totrecs.First();
            long totalRecords = row.GetValue<long>("count");
            int totpage = (int)Math.Ceiling((double)(totalRecords) / perpage);
            return totpage;
        }

        public IEnumerable<Product> ListAll(int page)
        {
            var perpage = 5;
            var offset = (page -1) * perpage;

            var prod = new List<Product>();
            var cql = $"SELECT * FROM core8.products";
            var rowSet = session.Execute(new SimpleStatement(cql));
            foreach (var row in rowSet)
            {
                prod.Add(new Product
                {
                    Id = row.GetValue<Guid>("id"),
                    Category = row.GetValue<string>("category"),
                    Descriptions = row.GetValue<string>("descriptions"),
                    Qty = row.GetValue<int>("qty"),
                    Unit = row.GetValue<string>("unit"),
                    CostPrice = row.GetValue<decimal>("costprice"),
                    SellPrice = row.GetValue<decimal>("sellprice"),   
                    SalePrice = row.GetValue<decimal>("saleprice"), 
                    ProductPicture = row.GetValue<string>("productpicture"),
                    AlertStocks = row.GetValue<int>("alertstocks"),
                    CriticalStocks = row.GetValue<int>("criticalstocks")
                });
            }

            return prod.Skip(offset).Take(perpage).ToList();
        }

        public async Task<int> TotPageSearch(int pg, string key) {
            var perpage = 5;
            var cql = new SimpleStatement("SELECT COUNT(*) FROM core8.product WHERE descriptions LIKE ?", key+"%");
            var totrecs = await session.ExecuteAsync(cql,"profile1").ConfigureAwait(false);

            var row = totrecs.First();
            long totalRecords = row.GetValue<long>("count");
            int totpage = (int)Math.Ceiling((double)(totalRecords) / perpage);
            return totpage;
        }

        public async Task<IEnumerable<Product>> SearchAll(int page,string key)
        {       
            var perpage = 5;
            var offset = (page -1) * perpage;

            try {
                var prod = new List<Product>();
                var stmt = new SimpleStatement("SELECT * FROM core8.product WHERE descriptions LIKE ?", key+"%");
                var rowSet = await session.ExecuteAsync(stmt, "profile1").ConfigureAwait(false);
                foreach (var row in rowSet)
                {
                    prod.Add(new Product
                    {
                        Id = row.GetValue<Guid>("id"),
                        Category = row.GetValue<string>("category"),
                        Descriptions = row.GetValue<string>("descriptions"),
                        Qty = row.GetValue<int>("qty"),
                        Unit = row.GetValue<string>("unit"),
                        CostPrice = row.GetValue<decimal>("costprice"),
                        SellPrice = row.GetValue<decimal>("sellprice"),   
                        SalePrice = row.GetValue<decimal>("saleprice"), 
                        ProductPicture = row.GetValue<string>("productpicture"),
                        AlertStocks = row.GetValue<int>("alertstocks"),
                        CriticalStocks = row.GetValue<int>("criticalstocks")
                    });
                }
                return prod.Skip(offset).Take(perpage).ToList();

            } catch(Exception ex) {
                throw new AppException(ex.Message);              
            }
        }

        public async Task CreateProduct(Product prod) {
            var preparedStatement = await session.PrepareAsync("SELECT * FROM core8.products WHERE descriptions = ? ALLOW FILTERING");
            var statement1 = preparedStatement.Bind(prod.Descriptions);
            var rowSet = await session.ExecuteAsync(statement1);
            var row = rowSet.FirstOrDefault();
            if (row is null) {                
                var prep = await session.PrepareAsync("INSERT INTO core8.products(id, category, descriptions, qty, unit, costprice, sellprice, saleprice, alertstocks, criticalstocks, productpicture, createdat) Values(?,?,?,?,?,?,?,?,?,?,?,?)");
                var bound = prep.Bind(prod.Id, prod.Category, prod.Descriptions, prod.Qty, prod.Unit, prod.CostPrice, prod.SellPrice, prod.SalePrice, prod.AlertStocks, prod.CriticalStocks, prod.ProductPicture, prod.CreatedAt);
                await session.ExecuteAsync(bound);
            } else {
                    throw new AppException("Product Description is already exists...");
            }
        }

        public void ProductUpdate(Product prods) {
            string findId = $"SELECT * FROM core8.products WHERE id = ?";
            var rowSet = session.Execute(new SimpleStatement(findId), prods.Id.ToString());
            foreach(var row in rowSet) {
                if (row is null)  {
                    throw new AppException("Product not found");
                }
            }

            DateTime now = DateTime.Now;
            string updateRecs = $"UPDATE core8.products SET category = ?, descriptions = ?, qty = ?, unit = ?, costPrice = ?, sellPrice = ?, salePrice = ?, alertStocks = ?, criticalStocks = ?, updatedAt = ? WHERE id = ?";
            PreparedStatement preparedStatement = session.Prepare(updateRecs);
            BoundStatement boundStatement = preparedStatement.Bind(prods.Category, prods.Descriptions, prods.Qty, prods.Unit, prods.CostPrice, prods.SellPrice, prods.SalePrice, prods.AlertStocks, prods.CriticalStocks, now, prods.Id.ToString());
            session.Execute(boundStatement);
        }

        public async Task<bool> ProductDelete(Guid id) {
                var preparedStatement = await session.PrepareAsync("SELECT * FROM core8.products WHERE id = ?");
                var statement1 = preparedStatement.Bind(id);
                var rowSet = await session.ExecuteAsync(statement1);
                var row = rowSet.FirstOrDefault();
                if (row is not null) {
                    var findRec = "DELETE FROM core8.products WHERE id = ? IF EXISTS";
                    var statement2 = new SimpleStatement(findRec, id);
                    await session.ExecuteAsync(statement2);
                    return true;
                } else {
                    return false;
                }
        }

        public void UpdateProdPicture(Guid id, string file) {
            string upd = $"SELECT * FROM core8.products WHERE id = ?";
            var rowSet = session.Execute(new SimpleStatement(upd), id.ToString());
            foreach(var row in rowSet) {
                if (row is null)  {
                    throw new AppException("Product not found");
                }
            }

            string uploadPic = "UPDATE core8.products SET productPicture = ? WHERE id = ?";
            PreparedStatement prep = session.Prepare(uploadPic);
            BoundStatement boundStm = prep.Bind(file, id.ToString());
            session.Execute(boundStm);
        }

        public IEnumerable<Product> GetAllRecords()
        {
        var records = new List<Product>();
        string cql = "SELECT * FROM core8.products;"; 
        var rowSet =  session.Execute(new SimpleStatement(cql));
        foreach (var row in rowSet)
        {
            records.Add(new Product 
            {
                Id = row.GetValue<Guid>("id"),
                Category = row.GetValue<string>("category"),
                Descriptions = row.GetValue<string>("descriptions"),
                Qty = row.GetValue<int>("qty"),
                Unit = row.GetValue<string>("unit"),
                CostPrice = row.GetValue<decimal>("costprice"),
                SellPrice = row.GetValue<decimal>("sellprice"),                
                SalePrice = row.GetValue<decimal>("saleprice"),
                ProductPicture = row.GetValue<string>("productpicture"),
                AlertStocks = row.GetValue<int>("alertstocks"),
                CriticalStocks = row.GetValue<int>("criticalstocks"),
            });
        }
        return records;
    }
        
        public Product GetProductById(Guid id) {
            var getRecs = new SimpleStatement("SELECT * FROM core8.products WHERE id = ?", id);
            var rowSet = session.Execute(getRecs);
            var prod = new Product();
            foreach(var row in rowSet)          
            {
                prod.Id = row.GetValue<Guid>("id");
                prod.Category = row.GetValue<string>("category");
                prod.Descriptions = row.GetValue<string>("descriptions");
                prod.Qty = row.GetValue<int>("qty");              
                prod.Unit = row.GetValue<string>("unit");
                prod.CostPrice = row.GetValue<decimal>("costprice");
                prod.SellPrice = row.GetValue<decimal>("sellprice");   
                prod.SalePrice = row.GetValue<decimal>("saleprice"); 
                prod.ProductPicture = row.GetValue<string>("productpicture"); 
                prod.AlertStocks = row.GetValue<int>("alertstocks"); 
                prod.CriticalStocks = row.GetValue<int>("criticalstocks"); 
            }
            return prod;   
    }

    }
}