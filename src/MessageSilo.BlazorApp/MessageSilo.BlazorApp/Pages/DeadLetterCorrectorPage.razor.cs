using MessageSilo.BlazorApp.Components.DeadLetterCorrector;
using MessageSilo.BlazorApp.Services;
using MessageSilo.Features.DeadLetterCorrector;
using MessageSilo.Shared.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;

namespace MessageSilo.BlazorApp.Pages
{
    public partial class DeadLetterCorrectorPage
    {
        [Inject]
        public IMessageSiloAPIService MessageSiloAPI { get; set; }

        [Parameter]
        public string Id { get; set; }

        public ConnectionSettingsDTO ConnectionSettings { get; set; }

        public List<CorrectedMessage> CorrectedMessages { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ConnectionSettings = await MessageSiloAPI.GetDeadLetterCorrector(Guid.Parse(Id));
            CorrectedMessages = await MessageSiloAPI.GetCorrectedMessages(Guid.Parse(Id));
        }
    }
}
