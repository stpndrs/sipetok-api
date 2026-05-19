namespace sipetok_api.dto.Respon
{
    class ResponValidation
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = "Validation error";
        public Dictionary<string, string[]> Errors { get; set; }

        public ResponValidation(Dictionary<string, string[]> errors)
        {
            this.Errors = errors;
        }
    }
}