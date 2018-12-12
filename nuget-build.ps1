$versionPrefix = "0.0.1"
$versionSuffix = "alpha-005"
dotnet pack .\src\Arragro.ObjectHistory.Web -c Release --version-suffix $versionSuffix
dotnet nuget push .\src\Arragro.ObjectHistory.Web\bin\Release\Arragro.ObjectHistory.Web.$versionPrefix-$versionSuffix.nupkg -s https://registry.arragro.com/repository/nuget-hosted/

dotnet pack .\src\Arragro.ObjectHistory.Client -c Release --version-suffix $versionSuffix
dotnet nuget push .\src\Arragro.ObjectHistory.Client\bin\Release\Arragro.ObjectHistory.Client.$versionPrefix-$versionSuffix.nupkg -s https://registry.arragro.com/repository/nuget-hosted/