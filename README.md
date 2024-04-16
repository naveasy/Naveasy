# Naveasy

This library is heavly based on the old [free and open sourced version PRISM library v8](https://github.com/PrismLibrary/Prism/releases/tag/DNF).

It was adapted to work with .NET MAUI and will be kept FREE and open source going forward.

It it works with:
- .NET MAUI
- Microsoft.Extensions.DependencyInjection
- MAUI NavigatonPage. There's No support for MAUI AppShell until MSFT trully fixes the following issues:

https://github.com/dotnet/maui/issues/7354
https://github.com/dotnet/maui/issues/21814
https://github.com/dotnet/maui/issues/21816

For now, to use it make sure all your views and view models follows the naming convention:
- Your pages must be named: {whatever}Page
- Your view models must be named: {whatever}PageViewModel
- Both page and VM MUST be on the same namespace.

In the next release we're going to remove these naming convension requirements.

To better understand how to use it follows the Sample on this repo.
