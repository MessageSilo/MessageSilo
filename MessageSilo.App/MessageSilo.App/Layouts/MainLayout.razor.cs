using Blazorise;
using Blazorise.Localization;

using Microsoft.AspNetCore.Components;

namespace MessageSilo.App.Layouts
{
    public partial class MainLayout
    {
        [Inject] protected ITextLocalizerService LocalizationService { get; set; }

        [CascadingParameter] protected Theme? Theme { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await SelectCulture("en-US");

            await base.OnInitializedAsync();
        }

        private Task SelectCulture(string name)
        {
            LocalizationService.ChangeLanguage(name);

            return Task.CompletedTask;
        }
    }
}