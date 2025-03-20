@echo off
setlocal

:: Set the TFM variable
set TFM=net8.0

:: Get the directory of this batch file
:: Since batch does not have an equivalent to "$BASH_SOURCE" and reading symlinks requires a different approach, we will assume you are calling this from the correct location
set DIR=%~dp0

:: Build the projects
dotnet build -c Release -f %TFM% %DIR%src\UltralightNet
dotnet build -c Release -f %TFM% %DIR%src\UltralightNet.AppCore

:: Copy the generated files
copy %DIR%src\UltralightNet\obj\Release\%TFM%\generated\Microsoft.Interop.LibraryImportGenerator\Microsoft.Interop.LibraryImportGenerator\LibraryImports.g.cs %DIR%src\UltralightNet\LibraryImportGenerator\LibraryImports.g.cs

:: Create the destination directory if it doesn't exist
if not exist %DIR%src\UltralightNet.AppCore\LibraryImportGenerator mkdir %DIR%src\UltralightNet.AppCore\LibraryImportGenerator

:: Copy the second generated file
copy %DIR%src\UltralightNet.AppCore\obj\Release\%TFM%\generated\Microsoft.Interop.LibraryImportGenerator\Microsoft.Interop.LibraryImportGenerator\LibraryImports.g.cs %DIR%src\UltralightNet.AppCore\LibraryImportGenerator\LibraryImports.g.cs

:: End script
endlocal
