using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Text.Json;

namespace simple_pos_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionApiController : ControllerBase
    {
        private SqliteConnection _connection = new SqliteConnection("Data source = simple_pos.db");

        // get all transaction
        // [HttpGet("GetTransaction")]
        // public async Task<IActionResult> GetTransaction()
        // {
        // }



        // create trancaction
        [HttpPost("SaveTransaction")]
        public async Task<IActionResult> SaveTransaction(Transaction transaction)
        {
            string transactionProductJson = JsonSerializer.Serialize(transaction.TransactionProduct);

            const string query = @"
                insert into Transactions (TotalAmount, TransactionProduct)
                values (@TotalAmount, @TransactionProduct);";

            var parameters = new
            {
                transaction.TotalAmount,
                TransactionProductList = transactionProductJson
            };

            await _connection.ExecuteAsync(query, parameters);
            return Ok(new { Message = "Transaction saved successfully." });
        }

    }
}