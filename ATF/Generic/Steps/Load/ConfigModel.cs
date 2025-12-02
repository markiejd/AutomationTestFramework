namespace Generic.Steps.Config
{
    public class ConfigModel
    {
        public string API { get; set; } = string.Empty;
        public bool verify_ssl { get; set; }
        public string request { get; set; } = string.Empty;
        public bool api_key { get; set; }
        public int user_number { get; set; }
        public int increment_rate { get; set; }
        public string run_time { get; set; } = string.Empty;
    }
}

