namespace Vcrm.Models.Logging;

// Mirrors Vcrm.Messaging.Models.LogSeverity in the Vcrm_Messaging repo - values must stay
// in sync since they're serialized as plain integers over the wire, not by name.
public enum CentralLogSeverity
{
    Trace = 0,
    Debug = 1,
    Information = 2,
    Warning = 3,
    Error = 4,
    Critical = 5
}
