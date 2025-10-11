using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using SPTarkov.Server.Core.Models.Common;

namespace TarkovTools.Web.Pages.Misc;

public partial class MongoIdGenerator : ComponentBase
{
    private List<string> _generatedIds = new();
    private HashSet<string> _copiedIds = new();
    private int _massGenerateCount = 1;
    
    private void GenerateMultipleIds()
    {
        for (int i = 0; i < _massGenerateCount; i++)
        {
            var newId = new MongoId().ToString();
            _generatedIds.Insert(0, newId);
        }
        
        StateHasChanged();
    }

    private void CopyToClipboard(string id)
    {
        _copiedIds.Add(id);
        Snackbar.Add("ID copied to clipboard", Severity.Success);
    }
}