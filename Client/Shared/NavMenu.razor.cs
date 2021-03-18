namespace BlazorIdentityBase.Client.Shared
{
    public partial class NavMenu
    {
        private bool CollapseNavMenu { get; set; }

        private string NavMenuCssClass => CollapseNavMenu ? "collapse" : null;

        private void ToggleNavMenu()
        {
            CollapseNavMenu = !CollapseNavMenu;
        }
    }
}
