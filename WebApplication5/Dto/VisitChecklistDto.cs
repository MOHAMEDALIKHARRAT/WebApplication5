namespace WebApplication5.Dto
{
    public class VisitChecklistDto
    {
        public string ObjectiveType { get; set; }
        public bool IsSelected { get; set; }
        public decimal? Encours { get; set; }
        public bool? ReglementPret { get; set; }
        public bool? Recupere { get; set; }
        public List<VisitOrderItemDto> OrderItems { get; set; } = new List<VisitOrderItemDto>();

    }
}
