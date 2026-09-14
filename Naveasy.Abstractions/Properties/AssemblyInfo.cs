using System.Runtime.CompilerServices;

// The navigation engines built on top of these abstractions share a few internal members
// (KnownInternalParameters, PageScopeService's constructor and GetNavigationParametersInternal).
// They are kept internal so they stay out of the public API surface of every Naveasy package.
[assembly: InternalsVisibleTo("Naveasy")]
[assembly: InternalsVisibleTo("Naveasy.Shell")]
