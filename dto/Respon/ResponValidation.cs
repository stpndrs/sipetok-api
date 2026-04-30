namespace sipetok_api.dto.Respon
{
    class ResponValidation
    {
        public bool success { get; set; } = false;
        public string message { get; set; } = "Validation error";
        public Dictionary<string, string[]> errors { get; set; } = new();
    }
}