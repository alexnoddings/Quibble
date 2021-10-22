using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Quibble.Client.Components.Authentication;

public partial class AuthForm
{
    [Parameter]
    public RenderFragment Title { get; set; } = default!;

    [Parameter]
    public RenderFragment Form { get; set; } = default!;

    [Parameter]
    public string? SubmitText { get; set; }

    [Parameter]
    public RenderFragment? Footer { get; set; }

    [Parameter]
    public List<string>? Errors { get; set; } = default!;

    [Parameter]
    public object Model { get; set; } = default!;

    [Parameter]
    public EventCallback<EditContext> OnSubmit { get; set; } = default!;

    private bool _isSubmitting;

    private async Task OnSubmitting(EditContext? editContext)
    {
        _isSubmitting = true;

        try
        {
            await OnSubmit.InvokeAsync(editContext);
        }
        catch (Exception)
        {
            _isSubmitting = false;
            StateHasChanged();
            throw;
        }

        _isSubmitting = false;
    }
}
