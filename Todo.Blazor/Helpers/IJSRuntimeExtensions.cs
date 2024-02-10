using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Todo.Blazor.Helpers
{
    public static class IJSRuntimeExtensions
    {
        private const string StorageKey = "authentication-token";

        public static ValueTask SaveAs(this IJSRuntime js, string fileName, byte[] content)
        {

            return js.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(content));
        }

        public static ValueTask DisplayMessage(this IJSRuntime js, string message)
        {
            return js.InvokeVoidAsync("Swal.fire", message);
        }

        public static ValueTask DisplayMessage(this IJSRuntime js, string title, string message, SweetAlertMessageType sweetAlertMessageType)
        {
            return js.InvokeVoidAsync("Swal.fire", title, message, sweetAlertMessageType.ToString());
        }

        public static ValueTask<bool> Confirm(this IJSRuntime js, string title, string message, SweetAlertMessageType sweetAlertMessageType)
        {
            return js.InvokeAsync<bool>("CustomConfirm", title, message, sweetAlertMessageType.ToString());
        }

        //     public static ValueTask SetInLocalStorage(this IJSRuntime js, string content)
        //=> js.InvokeVoidAsync(
        //    "localStorage.setItem",
        //    StorageKey, content
        //    );

        //     public static ValueTask<string> GetFromLocalStorage(this IJSRuntime js)
        //         => js.InvokeAsync<string>(
        //             "localStorage.getItem",
        //             StorageKey
        //             );

        //     public static ValueTask RemoveItem(this IJSRuntime js)
        //         => js.InvokeVoidAsync(
        //             "localStorage.removeItem",
        //             StorageKey);

        public static async ValueTask SetToken(this IJSRuntime js, string key, string content)
            => await js.InvokeVoidAsync("localStorage.setItem", StorageKey, content);

        public static async ValueTask<string> GetToken(this IJSRuntime js, string key)
            => await js.InvokeAsync<string>("localStorage.getItem", StorageKey);

        public static async ValueTask RemoveToken(this IJSRuntime js, string key)
            => await js.InvokeVoidAsync("localStorage.removeItem", StorageKey);
    }

    public enum SweetAlertMessageType
    {
        question, warning, error, success, info
    }
}
