using AutoMapper;
using Example.Application.Dto;
using Example.Domain.Entitys;
using Example.Domain.Exceptions;
using Example.Domain.Interfaces.Services;
using Example.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Example.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ProducesErrorResponseType(typeof(ExceptionDto))]
    public class ProductController : Controller
    {
        readonly protected IProductService _productService;
        readonly protected IMapper _mapper;

        public ProductController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;
        }

        /// <summary> Returns list of a product </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(Pagination<FindProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Pagination<FindProductDto>>> FindAllAsync([FromQuery] QueryData query)
        {
            var result = await _productService.FindAllAsync(query);
            var map = _mapper.Map<Pagination<FindProductDto>>(result);
            return Ok(map);
        }

        /// <summary> Returns detail of a product </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:int:min(1)}")]
        [ProducesResponseType(typeof(FindProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<FindProductDto>> FindByIdAsync(int id)
        {
            var result = await _productService.FindByIdAsync(id) ?? throw new NotFoundException("Product not found...");

            var map = _mapper.Map<FindProductDto>(result);

            return Ok(map);
        }

        /// <summary>
        /// Creates an product.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/v1/product
        ///     {        
        ///       "name": "Programação C#",
        ///       "Description": "Descrição",
        ///       "Price": 10.00,
        ///       "Quantity": 10,
        ///       "CategoryId": 1
        ///       "imageUrl": "Url", 
        ///     }
        /// </remarks>
        /// <param name="productDto"></param>   
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> InsertAsync([FromBody] ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            await _productService.InsertAsync(product);
            return StatusCode(StatusCodes.Status201Created);
        }

        /// <summary>
        /// Update an product.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT api/v1/product/{id}
        ///     {        
        ///       "name": "Programação C#",
        ///       "Description": "Descrição",
        ///       "Price": 10.00,
        ///       "Quantity": 10,
        ///       "CategoryId": 1
        ///       "imageUrl": "Url", 
        ///     }
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="productDto"></param>   
        [HttpPut]
        [Route("{id:int:min(1)}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] ProductDto productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            product.Id = id;
            await _productService.UpdateAsync(product);
            return StatusCode(StatusCodes.Status200OK);
        }

        /// <summary>
        /// Delete an product.
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
            await _productService.DeleteAsync(id);
            return StatusCode(StatusCodes.Status202Accepted);
        }
    }
}