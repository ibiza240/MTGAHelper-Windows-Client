namespace MTGAHelper.Entity
{
    public class ScraperDto
    {
        public string Type { get; set; }
        public string Id { get; set; }
        public string Format { get; set; }
        public bool IsByUser { get; set; }
        public bool IsActivated { get; set; }
        public int NbDecks { get; set; }
        public string Url { get; set; }

        public ScraperDto()
        {
        }

        public ScraperDto(ScraperType scraperType, bool isActivated, int nbDecks, string urlDeckList)
        {
            Type = scraperType.Type.ToString().ToLower();
            Id = scraperType.Id;
            Format = scraperType.Format.ToString().ToLower();
            IsByUser = scraperType.IsByUser;
            Url = urlDeckList ?? scraperType.Url;
            IsActivated = isActivated;
            NbDecks = nbDecks;
        }
    }
}
