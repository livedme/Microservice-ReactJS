using Blazored.LocalStorage;
using Microsoft.JSInterop;

namespace Todo.Blazor.Helpers
{
    public static class LocalStorageSetting: IJSRuntime
    {   
        private const string StorageKey = "authentication-token";        
        public static async Task SetTokenAsync(this IJSRuntime js, string content) => await js.InvokeVoidAsync("localStorage.setItem", StorageKey, content);

        public static async Task<string> GetTokenAsync(this IJSRuntime js) => await js.InvokeAsync<string>("localStorage.getItem", StorageKey);

        public static async Task RemoveTokenAsync(this IJSRuntime js) => await js.InvokeVoidAsync("localStorage.removeItem", StorageKey);

        public static Task<string?> GetTokenAsync()
        {
            throw new NotImplementedException();
        }

        //public async Task<string> GetTokenAsync() => await js.GetFromLocalStorage(StorageKey);
        //public async Task SetTokenAsync(string item) => await js.SetInLocalStorage(StorageKey, item);
        //public async Task RemoveTokenAsync() => await js.RemoveItem(StorageKey);
    }
}
