namespace sipetok_api.dto.Respon
{
    public class ResponData<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public ResponData(string message)
        {
            this.Success = false;
            this.Message = message;
        }

        public ResponData(bool success, string message, T data)
        {
            this.Data = data;
            this.Success = success;
            this.Message = message;
        }
    }
}