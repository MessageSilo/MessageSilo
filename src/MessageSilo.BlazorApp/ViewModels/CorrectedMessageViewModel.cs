using MessageSilo.Features.DeadLetterCorrector;

namespace MessageSilo.BlazorApp.ViewModels
{
    public class CorrectedMessageViewModel
    {
        public CorrectedMessage CorrectedMessage { get; set; }

        public bool IsVisible { get; set; }
    }
}
