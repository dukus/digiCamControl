using Microsoft.AspNetCore.Components;
using ScriptExternalObjectRestService.Components.ScriptExternalObjectDemo;

namespace ScriptExternalObjectRestService.Components.Pages
{
	partial class Home : IDisposable
	{
		private string RestServiceUrl
			=> NavigationManager.ToAbsoluteUri("/" + ScriptExternalObjectService.BASEURL_PATH).ToString();

		private static string? PropertyDescription (string propertyName)
			=> (from d in ScriptExternalObject.ObjectDescription
				where d.Name == propertyName
				select d).FirstOrDefault()?.Description;

		private static long ReadOnlyValue
		{
			get => ScriptExternalObject.Instance.ReadOnlyValue;
			set => ScriptExternalObject.Instance.SetReadOnlyValue(value);
		}

		[Inject]
		public NavigationManager NavigationManager { get; set; } = default!;

		public void Dispose ()
		{
			ScriptExternalObjectService.ObjectChanged -= ScriptExternalObjectChanged;
		}

		protected override void OnAfterRender (bool firstRender)
		{
			if (firstRender)
			{
				ScriptExternalObjectService.ObjectChanged += ScriptExternalObjectChanged;
			}
		}

		private void ScriptExternalObjectChanged (object? sender, EventArgs e)
		{
			InvokeAsync(StateHasChanged);
		}
	}
}
