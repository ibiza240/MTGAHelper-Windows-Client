namespace MTGAHelper.Web.Models.Response.Deck
{
    public class SimpleCustomDeckDto
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public SimpleCustomDeckDto(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}