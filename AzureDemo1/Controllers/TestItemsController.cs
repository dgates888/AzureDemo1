using AzureDemo1.Model;
using AzureDemo1.repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
namespace AzureDemo1.Controllers
{ 
    [ApiController]
    [Route("api/[controller]")]
    public class TestItemsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
         
        public TestItemsController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TestItem))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TestItem>> GetTestItemById(int id)
        {
            var testItem = await _context.TestItems.FindAsync(id);

            if (testItem == null)
            {
                return NotFound();
            }

            return Ok(testItem);
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TestItem>))]
        public async Task<ActionResult<IEnumerable<TestItem>>> GetTestItems()
        { 
            return await _context.TestItems.ToListAsync();
        }
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TestItem>> CreateTestItemViaPut([FromBody] TestItem testItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Ensure ID is treated as 0 so the database generates it.
            testItem.Id = 0;

            _context.TestItems.Add(testItem);
            await _context.SaveChangesAsync();

            // Assumes a corresponding GET action named "GetTestItemById" exists.
            return CreatedAtAction(nameof(GetTestItemById), new { id = testItem.Id }, testItem);
        }
        [HttpPost("update/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Success, no content body
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Invalid request (e.g., data validation failed)
        [ProducesResponseType(StatusCodes.Status404NotFound)]   // Item with 'id' was not found
        public async Task<IActionResult> UpdateTestItemViaPost(int id, [FromBody] TestItem updatedItem)
        {
            if (updatedItem.Id != 0 && id != updatedItem.Id)
            {
                return BadRequest("ID in URL must match ID in request body, or ID should be 0 or omitted in body.");
            }
            var existingItem = await _context.TestItems.FindAsync(id);

            // --- Handle Not Found ---
            if (existingItem == null)
            {
                // If no item found with that ID, return a 404 Not Found response
                return NotFound($"Item with ID {id} not found.");
            }
            existingItem.Name = updatedItem.Name;
            _context.Entry(existingItem).State = EntityState.Modified;
            try
            {
                // Asynchronously save the changes tracked by the DbContext to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Optional: Handle concurrency issues - occurs if the record was modified
                // or deleted by someone else after you fetched it.
                if (!await TestItemExists(id)) // Helper method to check if item still exists
                {
                    return NotFound($"Concurrency Error: Item with ID {id} may have been deleted.");
                }
                else
                {
                    // Log the error, return a 409 Conflict, or handle as appropriate
                    return Conflict("The item was modified by another user. Please refresh and try again.");
                }
            }

            // --- Return Success Response ---
            // Return HTTP 204 No Content, indicating success but no need to return the updated entity body.
            return NoContent();

        }
        private async Task<bool> TestItemExists(int id)
        {
            return await _context.TestItems.AnyAsync(e => e.Id == id);
        }

    }
}
