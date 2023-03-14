using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;


public class DebugManager
{
    private static readonly DebugManager instance = new DebugManager();
    private DebugManager() { }
    static DebugManager() { }
    public static DebugManager Instance
    {
        get
        {
            return instance;
        }
    }
    public Dialog newDialog { get; set; }
    public Dictionary<string, List<Dialog>> dialogs { get; set; } = new Dictionary<string, List<Dialog>>();

    public void AddDialog()
    {
        newDialog = new Dialog();
    }
}

