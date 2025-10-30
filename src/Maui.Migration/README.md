Maui migration scaffold
======================

This folder is a staging area for pages and components migrated from the React codebase.

Guidelines
- Add one View + ViewModel pair per feature under `src/Views` and `src/ViewModels` as you port.
- Keep platform-specific implementations in `Platforms/<OS>` and register via DI in `MauiProgram.cs`.

Example structure

  src\Maui.Migration\Views\DashboardPage.xaml
  src\Maui.Migration\ViewModels\DashboardViewModel.cs

This folder is intentionally empty of compiled artifacts and only contains guidance and placeholders.
