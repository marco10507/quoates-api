using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuotesAPI.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace QuotesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuotesController : Controller
    {
        private readonly Data.QuotesDbC _quotesDbContext;

        public QuotesController(Data.QuotesDbC quotesDbContext)
        {
            _quotesDbContext = quotesDbContext;
        }

        [HttpGet]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        public IActionResult Get(string sort)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            IQueryable quotes = sort switch {
                "acs" => _quotesDbContext.Quotes.Where(q => q.UserId == userId).OrderBy(q => q.PublicationDate),
                "desc" => _quotesDbContext.Quotes.Where(q => q.UserId == userId).OrderByDescending(q => q.PublicationDate),
                _ => _quotesDbContext.Quotes.Where(q => q.UserId == userId)
            };

            return Ok(quotes);
        }

        [HttpGet("[action]")]
        public IActionResult PagingQuote(int? pageNumber, int? pageSize) {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            var quotes = _quotesDbContext.Quotes.Where(q => q.UserId == userId);

            pageNumber ??= 1;
            pageSize ??= 5;

            return Ok(quotes.Skip((int)((pageNumber - 1) * pageSize)).Take((int)pageSize));
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult SearchQuote(string type) {
            IQueryable quotes = _quotesDbContext.Quotes.Where(q => q.Type.StartsWith(type));

            return Ok(quotes);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            Quote quote = _quotesDbContext.Quotes.Where(q => q.Id == id && q.UserId == userId).SingleOrDefault();

            if (quote == null) {
                return NotFound($"Quote with id {id} not found.");
            }else {
                return Ok(quote);
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody]Quote quote)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            quote.UserId = userId;
            _quotesDbContext.Quotes.Add(quote);
            _quotesDbContext.SaveChanges();

            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Quote quote)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            Quote queriedQuote = _quotesDbContext.Quotes.Where(q => q.Id == id && q.UserId == userId).SingleOrDefault();

            if (queriedQuote == null) {
                return NotFound($"Quote with id {id} not found.");
            }else{
                queriedQuote.Author = quote.Author;
                queriedQuote.Description = quote.Description;
                queriedQuote.Type = quote.Type;
                queriedQuote.PublicationDate = quote.PublicationDate;

                _quotesDbContext.Quotes.Update(queriedQuote);

                _quotesDbContext.SaveChanges();

                return Ok("Quote updated.");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            Quote quote = _quotesDbContext.Quotes.Where(q => q.Id == id && q.UserId == userId).SingleOrDefault();

            if (quote == null) {
                return NotFound($"Quote with id {id} not found.");
            }else {
                _quotesDbContext.Remove(quote);

                _quotesDbContext.SaveChanges();

                return Ok("Quote deleted.");
            }
        }
    }
}
