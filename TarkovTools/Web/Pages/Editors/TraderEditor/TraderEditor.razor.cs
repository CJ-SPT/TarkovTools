using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
namespace TarkovTools.Web.Pages.Editors.TraderEditor;

public partial class TraderEditor : ComponentBase
{
    [DefaultValue(null)]
    public Trader? SelectedTrader { get; set; }

    public delegate void OnTraderChangedHandler();
    public event OnTraderChangedHandler? OnTraderChanged;
    
    public void InvokeTraderChanged() => OnTraderChanged?.Invoke();

    public Dictionary<MongoId, Trader> GetTraders()
    {
        return DatabaseService.GetTables().Traders
            .Where(trader => trader.Value.Base.Id != "6864e812f9fe664cb8b8e152")
            .ToDictionary(x => x.Key, y => y.Value);
    }
}