namespace albums_api.Models
{
    public record Album(int Id, string Title, string Artist, double Price, string Image_url, int Year)
    {
        private static List<Album> _store = new List<Album>
        {
            new Album(1, "You, Me and an App Id", "Daprize", 10.99, "https://aka.ms/albums-daprlogo", 2021),
            new Album(2, "Seven Revision Army", "The Blue-Green Stripes", 13.99, "https://aka.ms/albums-containerappslogo", 2022),
            new Album(3, "Scale It Up", "KEDA Club", 13.99, "https://aka.ms/albums-kedalogo", 2022),
            new Album(4, "Lost in Translation", "MegaDNS", 12.99, "https://aka.ms/albums-envoylogo", 2023),
            new Album(5, "Lock Down Your Love", "V is for VNET", 12.99, "https://aka.ms/albums-vnetlogo", 2023),
            new Album(6, "Sweet Container O' Mine", "Guns N Probeses", 14.99, "https://aka.ms/albums-containerappslogo", 2024)
        };

        public static List<Album> GetAll() => new List<Album>(_store);

        public static Album? GetById(int id) => _store.FirstOrDefault(a => a.Id == id);

        public static Album Add(Album album)
        {
            var newId = _store.Count > 0 ? _store.Max(a => a.Id) + 1 : 1;
            var newAlbum = album with { Id = newId };
            _store.Add(newAlbum);
            return newAlbum;
        }

        public static Album? Update(int id, Album updated)
        {
            var index = _store.FindIndex(a => a.Id == id);
            if (index < 0) return null;
            var updatedAlbum = updated with { Id = id };
            _store[index] = updatedAlbum;
            return updatedAlbum;
        }

        public static bool Delete(int id)
        {
            var album = _store.FirstOrDefault(a => a.Id == id);
            if (album is null) return false;
            _store.Remove(album);
            return true;
        }

        public static List<Album> SearchByYear(int year) =>
            _store.Where(a => a.Year == year).ToList();
    }
}
