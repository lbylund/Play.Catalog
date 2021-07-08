using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Entities;
using Play.Catalog.Service.Repositories;

namespace Play.Catalog.Service.Controllers
{
  // https://localhost:5001/items
  [ApiController]
  [Route("items")]
  public class ItemsController : ControllerBase
  {
    private readonly IItemsRepository itemsRepository;

    public ItemsController(IItemsRepository itemsRepository)
    {
      this.itemsRepository = itemsRepository;
    }

    // private static readonly List<ItemDto> items = new()
    // {
    //   new ItemDto(Guid.NewGuid(), "Potion", "Restores a small amount of HP", 5, DateTimeOffset.UtcNow),
    //   new ItemDto(Guid.NewGuid(), "Antidote", "Cures posion", 7, DateTimeOffset.UtcNow),
    //   new ItemDto(Guid.NewGuid(), "Bronze sword", "Deals a small amount of damage", 20, DateTimeOffset.UtcNow)
    // };


    // GET /items
    [HttpGet]
    public async Task<IEnumerable<ItemDto>> GetAsync()
    {
      var items = (await itemsRepository.GetAllAsync())
                    .Select(item => item.AsDto());
      return items;
    }


    //GET /items/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
    {
      var item = await itemsRepository.GetAsync(id);

      if (item == null)
      {
        return NotFound();
      }

      return item.AsDto();
    }


    //POST /items
    [HttpPost]
    public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
    {
      var item = new Item
      {
        Name = createItemDto.Name,
        Description = createItemDto.Description,
        Price = createItemDto.Price,
        CreatedDate = DateTimeOffset.UtcNow
      };

      await itemsRepository.CreateAsync(item);

      return CreatedAtAction(nameof(GetByIdAsync), new { id = item.Id }, item);
    }


    //PUT /items/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
    {
      var existingItem = await itemsRepository.GetAsync(id);

      if (existingItem == null)
      {
        return NotFound();
      }

      existingItem.Name = updateItemDto.Name;
      existingItem.Description = updateItemDto.Description;
      existingItem.Price = updateItemDto.Price;

      await itemsRepository.UpdateAsync(existingItem);

      return NoContent();
    }


    // DELETE /items/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
      var item = await itemsRepository.GetAsync(id);

      if (item == null)
      {
        return NotFound();
      }

      await itemsRepository.RemoveAsync(item.Id);

      return NoContent();
    }
  }
}