using Blazorise;
using SBMonitor.BlazorApp.Shared;

namespace SBMonitor.BlazorApp.Pages
{
    public partial class Index
    {
        private Modal newConnectionModal;

        private ConnectionSettings connectionSettings;

        private Task ShowAddNewConnectionModal()
        {
            return newConnectionModal.Show();
        }
    }
}
