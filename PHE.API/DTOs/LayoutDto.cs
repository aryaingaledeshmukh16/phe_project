namespace PHE.API.DTOs
{
    public class LayoutDto
    {
        public string ApplicationType { get; set; } = string.Empty;

        public string Peth { get; set; } = string.Empty;

        public string Zone { get; set; } = string.Empty;

        public string PropertyNumber { get; set; } = string.Empty;

        public string LayoutAddress { get; set; } = string.Empty;

        public string ApprovedLayoutNumber { get; set; } = string.Empty;

        public DateTime? ApprovedLayoutDate { get; set; }
    }
}