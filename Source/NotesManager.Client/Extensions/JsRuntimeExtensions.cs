using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace NotesManager.Client.Extensions
{
    public static class JsRuntimeExtensions
    {
        public static ValueTask<object> SaveAs(this IJSRuntime js, string filename, byte[] data)
                => js.InvokeAsync<object>("saveAsFile", filename, Convert.ToBase64String(data));
    }
}
