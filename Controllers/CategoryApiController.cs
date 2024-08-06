using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.Data.Sqlite;

namespace simple_pos_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryApiController : ControllerBase
    {
        private SqliteConnection _connection = new SqliteConnection("Data source = simple_pos.db");

        // get all categories
        [HttpGet("GetCategories")]
        public async Task<IActionResult> GetCategories(){

            const string query = "Select * from Categories order by CategoryId desc";
            var result  = await _connection.QueryAsync<Category>(query);
            
            if(result.Count() == 0)
                return BadRequest("Bad Request");

            return Ok(result);
        }

        // get specified category
        [HttpGet("GetCategory")]
        public async Task<ActionResult<Category>> GetCategory(int categoryId){

            const string query = "Select * from Categories where CategoryId = @CategoryId LIMIT 1";
            var result  = await _connection.QueryAsync<Category>(query, new {CategoryId = categoryId});
            
            if(result == null)
                return BadRequest("Bad Request");

            return Ok(result);
        }

        // create category
        [HttpPost("SaveCategory")]
        public async Task<IActionResult> SaveCategoryAsync(Category category){
            const string query = "Insert into Categories (CategoryName) Values (@CategoryName); Select * from Categories order by CategoryId desc Limit 1";
            var result  = await _connection.QueryAsync<Category>(query, category);
            return Ok(result);
        }

        // update category
        [HttpPut("UpdateCategory")]
        public async Task<IActionResult> UpdateCategoryAsync(int id, Category category){
            const string query = "Update Categories set CategoryName = @CN where CategoryId = @Id; Select * from Categories where CategoryId = @Id limit 1 ";
            
            var result  = await _connection.QueryAsync<Category>(query, new {
                Id = id,
                CN = category.CategoryName,
            });

            return Ok(result);
        }

        // delete category
        [HttpDelete("DeleteCategory")]
        public async Task<IActionResult> DeleteCategory(int id){
            const string query = "Delete From Categories where CategoryId = @Id; ";
            await _connection.QueryAsync<Category>(query, new { Id = id,});
            return Ok();
        }

    }
}