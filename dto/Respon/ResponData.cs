namespace sipetok_api.dto.Respon
{
    class ResponData<T>
    {
        public bool status {get; set;}
        public List<String> message {get; set;} = new List<string>();
        public T? data {get; set;}
    }
}