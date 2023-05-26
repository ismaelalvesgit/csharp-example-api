using AutoMapper;
using Example.Application.Dto;
using Example.Domain.Entitys;
using Example.Domain.Exceptions;
using Example.Domain.Interfaces.Services;
using Example.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Example.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ProducesErrorResponseType(typeof(ExceptionDto))]
public class CategoryController : Controller
{
    readonly protected ICategoryService _categoryService;
    readonly protected IProducerService _producerService;
    readonly protected IMapper _mapper;

    public CategoryController(ICategoryService categoryService, IProducerService producerService, IMapper mapper)
    {
        _categoryService = categoryService;
        _producerService = producerService;
        _mapper = mapper;
    }

    /// <summary> Returns list of a category </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    [HttpGet]
    [ProducesResponseType(typeof(Pagination<FindCategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Pagination<FindCategoryDto>>> FindAllAsync([FromQuery] QueryData query)
    {
        var result = await _categoryService.FindAllAsync(query);
        var map = _mapper.Map<Pagination<FindCategoryDto>>(result);
        return Ok(map);
    }

    /// <summary> Returns detail of a category </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("{id:int:min(1)}")]
    [ProducesResponseType(typeof(FindCategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FindCategoryDto>> FindByIdAsync(int id)
    {
        var result = await _categoryService.FindByIdAsync(id) ?? throw new NotFoundException("Category not found..");
        var map = _mapper.Map<FindCategoryDto>(result);
        return Ok(map);
    }

    /// <summary>
    /// Creates an Category.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST api/v1/Category
    ///     {        
    ///       "name": "Livro",
    ///       "imageUrl": "Url", 
    ///     }
    /// </remarks>
    /// <param name="categoryDto"></param>   
    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> InsertAsync([FromBody] CategoryDto categoryDto)
    {
        var category = _mapper.Map<Category>(categoryDto);
        await _categoryService.InsertAsync(category);
        return StatusCode(StatusCodes.Status201Created);
    }

    /// <summary>
    /// Creates an Category.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST api/v1/Category
    ///     {        
    ///       "name": "Livro",
    ///       "imageUrl": "Url", 
    ///     }
    /// </remarks>
    /// <param name="categoryDto"></param>   
    [HttpPost("async")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> ProducerAsync([FromBody] CategoryDto categoryDto)
    {
        await _producerService.ProduceAsync(new ProducerData<CategoryDto>
        {
            Data = categoryDto,
            Topic = "Queuing.Example.Category"
        });
        return StatusCode(StatusCodes.Status200OK);
    }

    /// <summary>
    /// Update an Category.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     PUT api/v1/Category/{id}
    ///     {        
    ///       "name": "Livro",
    ///       "imageUrl": "Url", 
    ///     }
    /// </remarks>
    /// <param name="id"></param>
    /// <param name="categoryDto"></param>   
    [HttpPut]
    [Route("{id:int:min(1)}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UpdateAsync(int id, [FromBody] CategoryDto categoryDto)
    {
        var category = _mapper.Map<Category>(categoryDto);
        category.Id = id;
        await _categoryService.UpdateAsync(category);
        return StatusCode(StatusCodes.Status200OK);
    }

    /// <summary>
    /// Delete an Category.
    /// </summary>
    /// <param name="id"></param>
    [HttpDelete]
    [Route("{id:int:min(1)}")]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync(int id)
    {
        await _categoryService.DeleteAsync(id);
        return StatusCode(StatusCodes.Status202Accepted);
    }
}
