@echo off
set sourceFolder="C:\Users\UserA\Desktop\GuideRecipeBackend\GuideRecipeBackend\bin\Debug"
set destinationFolder1="C:\Users\UserA\Desktop\Vertex\GO - ServerCoordinator"
set destinationFolder2="C:\Users\UserA\Desktop\Vertex\GO - ServerDeviceCenter"

copy %sourceFolder%\GuideRecipeBackend.Plugin.pdb %destinationFolder1%
copy %sourceFolder%\GuideRecipeBackend.Plugin.dll %destinationFolder1%
copy %sourceFolder%\GuideRecipeBackend.Plugin.pdb %destinationFolder2%
copy %sourceFolder%\GuideRecipeBackend.Plugin.dll %destinationFolder2%

echo Files copied successfully to both destinations!