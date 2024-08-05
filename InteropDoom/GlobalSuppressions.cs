using System.Diagnostics.CodeAnalysis;

[assembly:SuppressMessage("P/Invoke",
    "SYSLIB1054", // Replace [DllImport] with [LibraryImport] to generate marshalling code at compile time
    Justification = "Marshalling is more complex than source gen can handle out of the box", // and i cba to manually write code to marshal structs
    Scope = "module"
)]
