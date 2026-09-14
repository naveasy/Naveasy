// Global using directives

global using System.Windows.Input;
global using Naveasy.Shell;
global using Naveasy.Shell.Samples.Models;
global using Naveasy.Shell.Samples.Views;
global using Naveasy.Shell.Samples.Views.Guard;
global using Naveasy.Shell.Samples.Views.Home;
global using Naveasy.Shell.Samples.Views.Login;
global using Naveasy.Shell.Samples.Views.Modal;
global using Naveasy.Shell.Samples.Views.Orders;
global using Naveasy.Shell.Samples.Views.Reports;

// MAUI has its own NavigationMode type, so the Naveasy one gets an alias - the same alias the
// Naveasy packages declare for themselves.
global using NavigationMode = Naveasy.Core.NavigationMode;

// This app lives inside the Naveasy.Shell namespace, which hides Microsoft.Maui.Controls.Shell.
// An app with its own namespace can simply write Shell.
global using MauiShell = Microsoft.Maui.Controls.Shell;
