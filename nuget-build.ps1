$versionPrefix = "0.0.1"
$versionSuffix = "alpha-001"
dotnet pack .\src\Arragro.ObjectHistory.RazorClassLib -c Release --version-prefix $versionPrefix --version-suffix $versionSuffix
dotnet nuget push .\src\Arragro.ObjectHistory.RazorClassLib\bin\Release\Arragro.ObjectHistory.RazorClassLib.$versionPrefix-$versionSuffix.nupkg -s https://registry.arragro.com/repository/nuget-hosted/