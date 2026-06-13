namespace sipetok_api.dto.Respon
{
    public class ResponData<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public ResponData() { }
        public ResponData(bool success, string message)
        {
            this.Success = success;
            this.Message = message;
        }

        public ResponData(bool success, T data, string message)
        {
            this.Data = data;
            this.Success = success;
            this.Message = message;
        }
    }
}