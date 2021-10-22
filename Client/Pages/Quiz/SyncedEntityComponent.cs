using Microsoft.AspNetCore.Components;

namespace Quibble.Client.Pages.Quiz;

public abstract class SyncedEntityComponent : ComponentBase
{
    protected int LastStateStamp { get; set; } = 0;

    protected override void OnAfterRender(bool firstRender)
    {
        LastStateStamp = CalculateStateStamp();
    }

    protected Task OnUpdatedAsync() => InvokeAsync(StateHasChanged);

    protected abstract int CalculateStateStamp();

    protected override bool ShouldRender()
    {
        var currentStateStamp = CalculateStateStamp();
        if (currentStateStamp == LastStateStamp)
            return false;

        LastStateStamp = currentStateStamp;
        return true;
    }
}
