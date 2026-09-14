using System.Runtime.CompilerServices;

// These types used to live in the Naveasy assembly and were moved to Naveasy.Abstractions so that
// both navigation engines (Naveasy and Naveasy.Shell) can share them. The forwarders keep already
// compiled consumers of Naveasy working without a recompilation.
[assembly: TypeForwardedTo(typeof(Naveasy.INavigationParameters))]
[assembly: TypeForwardedTo(typeof(Naveasy.NavigationParameters))]
[assembly: TypeForwardedTo(typeof(Naveasy.ExtensionsOfNavigationParameters))]
[assembly: TypeForwardedTo(typeof(Naveasy.ParamExtensions))]
[assembly: TypeForwardedTo(typeof(Naveasy.INavigationResult))]
[assembly: TypeForwardedTo(typeof(Naveasy.NavigationResult))]
[assembly: TypeForwardedTo(typeof(Naveasy.IInitialize))]
[assembly: TypeForwardedTo(typeof(Naveasy.IInitializeAsync))]
[assembly: TypeForwardedTo(typeof(Naveasy.INavigatedAware))]
[assembly: TypeForwardedTo(typeof(Naveasy.IPageLifecycleAware))]
[assembly: TypeForwardedTo(typeof(Naveasy.IPageDialogService))]
[assembly: TypeForwardedTo(typeof(Naveasy.BindableBase))]
[assembly: TypeForwardedTo(typeof(Naveasy.Core.NavigationMode))]
[assembly: TypeForwardedTo(typeof(Naveasy.Core.INavigationParametersInternal))]
[assembly: TypeForwardedTo(typeof(Naveasy.Core.NavigationAttached))]
[assembly: TypeForwardedTo(typeof(Naveasy.Core.IPageScopeService))]
[assembly: TypeForwardedTo(typeof(Naveasy.Core.PageScopeService))]
[assembly: TypeForwardedTo(typeof(Naveasy.Extensions.NavigationParametersExtensions))]
[assembly: TypeForwardedTo(typeof(Naveasy.Behaviors.BehaviorBase<>))]
[assembly: TypeForwardedTo(typeof(Naveasy.Behaviors.PageLifecycleAwareBehavior))]
