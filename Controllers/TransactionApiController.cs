using Microsoft.AspNetCore.Mvc;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Text.Json;
using simple_pos_backend.Models;

namespace simple_pos_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionApiController : ControllerBase
    {
        private readonly SqliteConnection _connection = new("Data source = simple_pos.db");

        // get all transaction
        [HttpGet("GetTransaction")]
        public async Task<IActionResult> GetTransaction()
        {
            const string query = "SELECT TransactionId, TransactionDate, TotalAmount, TransactionProduct FROM Transactions";

            var transactions = await _connection.QueryAsync<TransactionDTO>(query);

            var result = transactions.Select(t => new Transaction
            {
                TransactionId = t.TransactionId,
                TransactionDate = DateTime.Parse(t.TransactionDate),
                TotalAmount = t.TotalAmount,
                TransactionProduct = JsonSerializer.Deserialize<List<TProduct>>(t.TransactionProduct)
            }).ToList();

            return Ok(result);
        }

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
                TransactionProduct = transactionProductJson
            };

            await _connection.ExecuteAsync(query, parameters);
            return Ok(new { Message = "Transaction saved successfully." });
        }


    }
}