$version = "5.0.0-alpha-202010401.1"
$ErrorActionPreference = "Stop"

$paths = @(
	".\src\Arragro.ObjectHistory.Web",
	".\src\Arragro.ObjectHistory.Client"
)

function executeSomething {
	param($something)
	$something
	if($LASTEXITCODE -ne 0)
	{
		exit
	}
}

foreach ($path in $paths) {
	$bin = "$($path)\bin"
	$obj = "$($path)\obj"
	if([System.IO.File]::Exists($bin)){
		Remove-Item $bin -Force -Recurse
	}
	if([System.IO.File]::Exists($obj)){
		Remove-Item $obj -Force -Recurse
	}
}

# executeSomething(dotnet test .\tests\ArragroCMS.IntegrationTests -c Debug )

dotnet clean

foreach ($path in $paths) {
	dotnet pack $path -c Debug /p:Version=$version --include-symbols --include-source
	$projectName = $path.Replace(".\src\", "").Replace(".\providers\", "")
	executeSomething(dotnet nuget push $path\bin\Debug\$($projectName).$version.nupkg -s https://registry.arragro.com/repository/nuget-hosted/)
}
