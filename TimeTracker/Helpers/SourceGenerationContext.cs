using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TimeTracker.Data;
using TimeTracker.Forms;
using TimeTracker.Pages;

namespace TimeTracker.Helpers;

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(State))]
[JsonSerializable(typeof(CloseTrackingForm.CloseTrackingFormData))]
[JsonSerializable(typeof(CloseTrackingForm.CloseTrackingFormResponse))]
internal partial class SourceGenerationContext : JsonSerializerContext 
{ 
}
