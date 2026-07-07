namespace sipetok_api.Utils
{
    public class TabelDriven
    {
        public int Key { get; set; }
        public string Label { get; set; }

        public TabelDriven(int key, string label)
        {
            this.Key = key;
            this.Label = label;
        }
    }
}
