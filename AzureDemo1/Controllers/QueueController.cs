using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using AzureDemo1.Model;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace AzureDemo1.Controllers
{
    [ApiController]
    public class QueueController : ControllerBase
    {
        private readonly QueueClient _queueClient;

        // Inject IConfiguration via constructor
        public QueueController(QueueClient queueClient)
        {
            _queueClient = queueClient ?? throw new ArgumentNullException(nameof(queueClient));
        } 
        [HttpPost("/api/Queue/QueueMessageModel")]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> PostMessageToQueue([FromBody] QueueMessageModel queueMessageModel)
        {

            queueMessageModel.CorrelationId = Guid.NewGuid();
            
            try
            {
                // Use the injected _queueClient directly
                await _queueClient.CreateIfNotExistsAsync(); // Still good practice
                string messageBody = JsonSerializer.Serialize(queueMessageModel); 
                //string messageBody = JsonSerializer.Serialize("Test API");
                var bytes = Encoding.UTF8.GetBytes(messageBody);
                var sendReceipt = await _queueClient.SendMessageAsync(Convert.ToBase64String(bytes));
                //var sendReceipt = await _queueClient.SendMessageAsync(messageBody);
                Console.WriteLine(sendReceipt.Value.MessageId); 
                return Accepted(new { MessageId = sendReceipt.Value.MessageId.ToString() });
            }
            catch(Exception ex) 
            {
                return Problem(ex.Message,"",500 );
            }
        }
    }
    
}
