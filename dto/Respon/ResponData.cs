namespace sipetok_api.dto.Respon
{
    class ResponData<T>
    {
        public bool success {get; set;}
        public string message {get; set;}
        public T? data {get; set;}
    }
}