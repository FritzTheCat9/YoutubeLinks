using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace YoutubeLinks.Blazor.Pages.Error;

public partial class CustomValidator : ComponentBase
{
    private ValidationMessageStore _validationMessageStore;

    [CascadingParameter] private EditContext EditContext { get; set; }

    protected override void OnInitialized()
    {
        if (EditContext is null)
        {
            throw new Exception();
        }

        _validationMessageStore = new ValidationMessageStore(EditContext);

        EditContext.OnValidationRequested += ValidationRequested;
        EditContext.OnFieldChanged += FieldChanged;
    }

    public void DisplayErrors(Dictionary<string, List<string>> errors)
    {
        foreach (var error in errors)
            _validationMessageStore.Add(new FieldIdentifier(EditContext.Model, error.Key), error.Value);

        EditContext.NotifyValidationStateChanged();
    }

    private void ValidationRequested(object sender, ValidationRequestedEventArgs e)
    {
        _validationMessageStore.Clear();

        EditContext.NotifyValidationStateChanged();
    }

    private void FieldChanged(object sender, FieldChangedEventArgs e)
    {
        _validationMessageStore.Clear(e.FieldIdentifier);

        EditContext.NotifyValidationStateChanged();
    }
}