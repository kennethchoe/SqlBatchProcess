nuget pack SqlBatchProcess\SqlBatchProcess.csproj -Prop Configuration=Release
nuget push SqlBatchProcess.*.nupkg
del SqlBatchProcess.*.nupkg
pause