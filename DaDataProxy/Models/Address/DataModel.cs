namespace DaDataProxy.Models.Address
{
    public class DataModel
    {
        public List<AddressData> Suggestions { get; set; }
    }

    public class AddressData
    {
        public string Value { get; set; }
    }
}