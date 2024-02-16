namespace LocationSearch.Model
{
    public class Location
    {
        public string? Name { get; set; }
        public Dictionary<string, string[]>? Availability { get; set; }
    }
}
