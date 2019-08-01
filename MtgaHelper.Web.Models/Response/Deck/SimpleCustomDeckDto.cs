namespace MTGAHelper.Web.UI.Model.Response.Dto
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
