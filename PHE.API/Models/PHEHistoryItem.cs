namespace PHE.API.Models
{
    public class PHEHistoryItem
    {
        public int logId { get; set; }
        public string applicationNo { get; set; } = string.Empty;
        public string? role { get; set; }
        public string? userCode { get; set; }
        public string? userName { get; set; }
        public string? scrutinyStatus { get; set; }
        public string? applicationStatus { get; set; }
        public string? layoutYesNo { get; set; }
        public string? remark { get; set; }
        public DateTime? entryDate { get; set; }
    }
}
