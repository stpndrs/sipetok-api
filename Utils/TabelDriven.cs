namespace sipetok_api.Utilis
{
    public class TabelDriven
    {
        public int key { get; set; }
        public string label { get; set; }

        public TabelDriven(int key, string label)
        {
            this.key = key;
            this.label = label;
        }
    }
}
