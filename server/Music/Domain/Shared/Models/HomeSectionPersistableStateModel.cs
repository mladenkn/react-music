namespace Music.Domain.Shared.Models
{
    public class HomeSectionPersistableStateModel
    {
        public HomeSectionOptionsModel Options { get; set; }

        public string SelectedTrackYoutubeId { get; set; }

        public string CurrentTrackYoutubeId { get; set; }
    }
}
