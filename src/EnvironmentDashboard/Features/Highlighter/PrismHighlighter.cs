using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace EnvironmentDashboard.Features.Highlighter;

public class PrismHighlighter
{
    private readonly IJSRuntime jsRuntime;

    public PrismHighlighter(IJSRuntime jsRuntime)
    {
        this.jsRuntime = jsRuntime;
    }

    public async Task HighlightAllAsync()
    {
        await jsRuntime.InvokeVoidAsync("highlightAll");
    }

    public async Task<MarkupString> HighlightAsync(string code, string language = "json")
    {
        var highlightedString = await jsRuntime.InvokeAsync<string>("highlightJson", code); //, language);
        return new MarkupString(highlightedString);
    }
}
