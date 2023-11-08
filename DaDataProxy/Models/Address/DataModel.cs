namespace DaDataProxy.Models.Address
{
    public class DataModel
    {
        public List<AdressSuggestions> Suggestions { get; set; }
    }

    public class AdressSuggestions
    {
        public string Value { get; set; }
    }
}