using Xunit;
using albums_api.Controllers;
using albums_api.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace albums_api.Tests.Controllers
{
    public class AlbumControllerTests
    {
        private readonly AlbumController _controller;

        public AlbumControllerTests()
        {
            _controller = new AlbumController();
        }

        // GET /albums
        [Fact]
        public void Get_ReturnsOkWithAlbums()
        {
            var result = _controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var albums = Assert.IsAssignableFrom<IEnumerable<Album>>(okResult.Value);
            Assert.NotEmpty(albums);
        }

        // GET /albums/sorted?sortBy=title
        [Fact]
        public void GetSorted_ByTitle_ReturnsTitleAscending()
        {
            var result = _controller.GetSorted("title");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var albums = Assert.IsAssignableFrom<List<Album>>(okResult.Value);
            var titles = albums.Select(a => a.Title).ToList();
            Assert.Equal(titles.OrderBy(t => t).ToList(), titles);
        }

        [Fact]
        public void GetSorted_ByArtist_ReturnsArtistAscending()
        {
            var result = _controller.GetSorted("artist");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var albums = Assert.IsAssignableFrom<List<Album>>(okResult.Value);
            var artists = albums.Select(a => a.Artist).ToList();
            Assert.Equal(artists.OrderBy(a => a).ToList(), artists);
        }

        [Fact]
        public void GetSorted_ByPrice_ReturnsPriceAscending()
        {
            var result = _controller.GetSorted("price");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var albums = Assert.IsAssignableFrom<List<Album>>(okResult.Value);
            var prices = albums.Select(a => a.Price).ToList();
            Assert.Equal(prices.OrderBy(p => p).ToList(), prices);
        }

        [Fact]
        public void GetSorted_UnknownSortBy_DefaultsToTitleAscending()
        {
            var result = _controller.GetSorted("unknown");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var albums = Assert.IsAssignableFrom<List<Album>>(okResult.Value);
            var titles = albums.Select(a => a.Title).ToList();
            Assert.Equal(titles.OrderBy(t => t).ToList(), titles);
        }

        // GET /albums/search?year=...
        [Fact]
        public void Search_ByYear_ReturnsMatchingAlbums()
        {
            var result = _controller.Search(2023);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var albums = Assert.IsAssignableFrom<List<Album>>(okResult.Value);
            Assert.All(albums, a => Assert.Equal(2023, a.Year));
        }

        [Fact]
        public void Search_ByYearWithNoMatches_ReturnsEmptyList()
        {
            var result = _controller.Search(1900);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var albums = Assert.IsAssignableFrom<List<Album>>(okResult.Value);
            Assert.Empty(albums);
        }

        // POST /albums
        [Fact]
        public void Create_ValidAlbum_ReturnsCreatedWithNewId()
        {
            var newAlbum = new Album(0, "New Album", "New Artist", 11.99, "https://example.com/img.jpg", 2025);

            var result = _controller.Create(newAlbum);

            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var created = Assert.IsType<Album>(createdResult.Value);
            Assert.True(created.Id > 0);
            Assert.Equal("New Album", created.Title);
            Assert.Equal(2025, created.Year);
        }

        // PUT /albums/{id}
        [Fact]
        public void Update_ExistingAlbum_ReturnsOkWithUpdatedData()
        {
            var updated = new Album(0, "Updated Title", "Updated Artist", 15.99, "https://example.com/img.jpg", 2026);

            var result = _controller.Update(1, updated);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var album = Assert.IsType<Album>(okResult.Value);
            Assert.Equal(1, album.Id);
            Assert.Equal("Updated Title", album.Title);
        }

        [Fact]
        public void Update_NonExistentAlbum_ReturnsNotFound()
        {
            var updated = new Album(0, "Ghost", "Nobody", 0.0, "", 2000);

            var result = _controller.Update(9999, updated);

            Assert.IsType<NotFoundResult>(result);
        }

        // DELETE /albums/{id}
        [Fact]
        public void Delete_NonExistentAlbum_ReturnsNotFound()
        {
            var result = _controller.Delete(9999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
