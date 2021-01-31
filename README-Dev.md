## Publish to nuget.org
Both nuget.exe and nuget.org are full of quirkes. The following commands are tested and seem to work
```
cd directory_containing_nuget_and_snuget_package
nuget push the_nuget_package -Source https://api.nuget.org/v3/index.json
```

Notes:   
1. nuget push suppose to push both the nupkg and the snupkg packages in one go
2. nuget push will **NOT** report error when the snupkg is **NOT** pushed
3. nuget push will only push snupkg when the source is https://api.nuget.org/v3/index.json which supports snupkg
4. nuget push will only push snupkg when it is executed within the directory containing the snupkg package according to reports
