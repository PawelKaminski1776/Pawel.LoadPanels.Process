using NServiceBus;
namespace BowlingSys.Entities.UserDBEntities
{
    public class GetPanelView : IMessage
    {
        public string Title { get; set; } 
        public string Image { get; set; } 
        public string Description { get; set; } 
        public string ExtendedDescription { get; set; } 
        public string Price { get; set; } 
    }

    public class GetViews : IMessage
    {
        public List<GetPanelView> Results { get; set; }
    }
}