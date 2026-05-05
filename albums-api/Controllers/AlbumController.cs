using albums_api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace albums_api.Controllers
{
    [Route("albums")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        // GET: api/album
        [HttpGet]
        public IActionResult Get()
        {
            var albums = Album.GetAll();

            return Ok(albums);
        }

        // GET: albums/sorted?sortBy=title|artist|price
        [HttpGet("sorted")]
        public IActionResult GetSorted([FromQuery] string sortBy = "title")
        {
            var albums = Album.GetAll();
            List<Album> sortedAlbums = sortBy.ToLower() switch
            {
                "artist" => albums.OrderBy(a => a.Artist).ToList(),
                "price" => albums.OrderBy(a => a.Price).ToList(),
                _ => albums.OrderBy(a => a.Title).ToList(),
            };
            return Ok(sortedAlbums);
        }

    }
}
