using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace simple_rest_api
{

    [ApiController]
    [Route("api/articles")]
    public class ArticlesController : Controller
    {
        private IRepository _repository;

        public ArticlesController(IRepository repository)
        {
            // Console.WriteLine("Sample debug output");
            _repository = repository;
        }       

        [HttpGet("{id:guid}")]
        public IActionResult GetArticle(Guid id)
        {
            var article = _repository.Get(id);
            if (article == null)
            {
                return StatusCode(StatusCodes.Status404NotFound, "Article not Found");
            }

            return this.Json(article);
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<IActionResult> CreateArticleAsync([FromBody] Article article)
        {
            try
            {
               
                article.Id = _repository.Create(article);

                this.HttpContext.Response.Headers.Add("Location", "/api/articles/" + article.Id);

                await HttpResponseWritingExtensions.WriteAsync(this.HttpContext.Response, article.Id.ToString());

            }
            catch (Exception)
            {                
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            return StatusCode(StatusCodes.Status201Created, Json(article));

        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteArticle(Guid id)
        {
            try
            {

                bool deleted = _repository.Delete(id);
                if (deleted)
                {
                    return StatusCode(StatusCodes.Status200OK);
                    
                }
                else
                {
                    return StatusCode(StatusCodes.Status404NotFound, "Article not Found");
                }
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

        }

        [HttpPut("{id:guid}")]
        public IActionResult UpdateArticle(Guid id, [FromBody] Article article)
        {
            try
            {      

                bool found = _repository.Update(article);
                if (found)
                {
                    return StatusCode(StatusCodes.Status200OK);
                }
                else
                {
                    return StatusCode(StatusCodes.Status404NotFound, "Article not Found");

                }

            }
            catch (Exception)
            {                
                return StatusCode(StatusCodes.Status400BadRequest);
            }
        }
    }
}


public interface IRepository
{
    // Returns a found article or null.
    Article Get(Guid id);
    // Creates a new article and returns its identifier.
    // Throws an exception if a article is null.
    // Throws an exception if a title is null or empty.
    Guid Create(Article article);
    // Returns true if an article was deleted or false if it was not possible to find it.
    bool Delete(Guid id);
    // Returns true if an article was updated or false if it was not possible to find it.
    // Throws an exception if an articleToUpdate is null.
    // Throws an exception or if a title is null or empty.
    bool Update(Article articleToUpdate);
}


public class Article
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Text { get; set; }
}



