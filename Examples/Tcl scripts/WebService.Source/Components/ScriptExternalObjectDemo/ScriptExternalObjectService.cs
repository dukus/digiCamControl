using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScriptExternalObjectRestService.Components.ScriptExternalObjectDemo
{
	/// <summary>
	/// Service to access the <see cref="ScriptExternalObject"/> by digiCamControl.
	/// </summary>
	[ApiController]
	[Route(BASEURL_PATH)]
	public sealed class ScriptExternalObjectService : Controller
	{
		public const string BASEURL_PATH = "api/dcc";

		public static event EventHandler? ObjectChanged;

		// OPTIONS: description of the scripted object
		[HttpOptions]
		public JsonResult Index ()
		{
			return new JsonResult(ScriptExternalObject.ObjectDescription, _options);
		}
		private static JsonSerializerOptions _options = new()
		{
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			WriteIndented = true,
		};

		// GET: BooleanValue
		[HttpGet(nameof(ScriptExternalObject.BooleanValue))]
		public ActionResult BooleanValue ()
		{
			return Json(ScriptExternalObject.Instance.BooleanValue);
		}

		// SET: BooleanValue
		[HttpPut(nameof(ScriptExternalObject.BooleanValue))]
		public ActionResult BooleanValue ([FromBody] bool value)
		{
			ScriptExternalObject.Instance.BooleanValue = value;
			ObjectChanged?.Invoke(this, EventArgs.Empty);
			return Ok();
		}

		// GET: LongValue
		[HttpGet(nameof(ScriptExternalObject.LongValue))]
		public ActionResult LongValue ()
		{
			return Json(ScriptExternalObject.Instance.LongValue);
		}

		// SET: LongValue
		[HttpPut(nameof(ScriptExternalObject.LongValue))]
		public ActionResult LongValue ([FromBody] long value)
		{
			ScriptExternalObject.Instance.LongValue = value;
			ObjectChanged?.Invoke(this, EventArgs.Empty);
			return Ok();
		}

		// GET: StringValue
		[HttpGet(nameof(ScriptExternalObject.StringValue))]
		public ActionResult StringValue ()
		{
			return Json(ScriptExternalObject.Instance.StringValue.ToString());
		}

		// SET: StringValue
		[HttpPut(nameof(ScriptExternalObject.StringValue))]
		public ActionResult StringValue ([FromBody] string value)
		{
			ScriptExternalObject.Instance.StringValue = value;
			ObjectChanged?.Invoke(this, EventArgs.Empty);
			return Ok();
		}

		// GET: EnumValue
		[HttpGet(nameof(ScriptExternalObject.EnumValue))]
		public ActionResult EnumValue ()
		{
			return Json(ScriptExternalObject.Instance.EnumValue.ToString());
		}

		// SET: EnumValue
		[HttpPut(nameof(ScriptExternalObject.EnumValue))]
		public ActionResult EnumValue ([FromBody] string value)
		{
			ScriptExternalObject.Instance.EnumValue = Enum.Parse<ScriptExternalObject.EnumValueType>(value);
			ObjectChanged?.Invoke(this, EventArgs.Empty);
			return Ok();
		}

		// GET: ReadOnlyValue
		[HttpGet(nameof(ScriptExternalObject.ReadOnlyValue))]
		public ActionResult ReadOnlyValue ()
		{
			return Json(ScriptExternalObject.Instance.ReadOnlyValue);
		}

		// SET: WriteOnlyValue
		[HttpPut(nameof(ScriptExternalObject.WriteOnlyValue))]
		public ActionResult WriteOnlyValue ([FromBody] string value)
		{
			ScriptExternalObject.Instance.WriteOnlyValue = value;
			ObjectChanged?.Invoke(this, EventArgs.Empty);
			return Ok();
		}

		// POST: PressButtonPressButton
		[HttpPost(nameof(ScriptExternalObject.PressButton))]
		public ActionResult PressButton ()
		{
			ScriptExternalObject.Instance.PressButton();
			ObjectChanged?.Invoke(this, EventArgs.Empty);
			return Ok();
		}

		// POST: EventRaised
		[HttpPost(nameof(ScriptExternalObject.EventRaised))]
		public ActionResult EventRaised ()
		{
			var value = ScriptExternalObject.Instance.EventRaised;
			ObjectChanged?.Invoke(this, EventArgs.Empty);
			return Json(value);
		}
	}
}
