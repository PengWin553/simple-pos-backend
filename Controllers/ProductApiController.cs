using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Authorization;
using simple_pos_backend.Models;

namespace simple_pos_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductApiController : ControllerBase
    {
        private SqliteConnection _connection = new SqliteConnection("Data source = simple_pos.db");

        // get all products
        [HttpGet("GetProducts")]
        public async Task<IActionResult> GetProducts(){

            const string query = "Select * from Products order by ProductId desc";
            var result  = await _connection.QueryAsync<Product>(query);
            
            if(result.Count() == 0)
                return BadRequest("Bad Request");

            return Ok(result);
        }

        // get specified product
        [HttpGet("GetProduct{productId}")]
        public async Task<ActionResult<Product>> GetProduct(int productId){

            const string query = "Select * from Products where ProductId = @ProductId LIMIT 1";
            var result  = await _connection.QueryAsync<Product>(query, new {ProductId = productId});
            
            if(result == null)
                return BadRequest("Bad Request");

            return Ok(result);
        }

        // create product
        [HttpPost("SaveProduct")]
        public async Task<IActionResult> SaveProductAsync(Product product){
            const string query = "Insert into Products (ProductName, Price, Stock, Unit, Sku, CategoryId) Values (@ProductName, @Price, @Stock, @Unit, @Sku, @CategoryId); Select * from Products order by ProductId desc Limit 1";
            var result  = await _connection.QueryAsync<Product>(query, product);
            return Ok(result);
        }

        // update product
        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> UpdateProductAsync(int id, Product product){
            const string query = "Update Products set ProductName = @productName, Price = @price, Stock = @stock, Unit = @unit, CategoryId = @categoryId where ProductID = @productID; Select * from Products where ProductID = @productID limit 1";
            
            var result  = await _connection.QueryAsync<Product>(query, new {
                productID = id,
                productName = product.ProductName,
                price = product.Price,
                stock = product.Stock,
                unit = product.Unit,
                categoryId = product.CategoryId
            });

            return Ok(result);
        }

        // delete product
        [HttpDelete("DeleteProduct")]
        public async Task<IActionResult> DeleteProduct(int id){
            const string query = "Delete From Products where ProductId = @productId; ";
            await _connection.QueryAsync<Product>(query, new { productId = id });
            return Ok();
        }
    }
}