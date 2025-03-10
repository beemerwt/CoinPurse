set "FILES=manifest.json icon.png README.md ingame.png"
set "RELEASE_DIR=releases"
set "ASSETS_DIR=Assets"
set "DLL_PATH=bin\Debug\net462\CoinPurse.dll"

set "TEMP_DIR=CoinPurse"
set "DLL_TEMP=CoinPurse.dll"

set "ZIP_FILE=CoinPurse-1.0.1.zip"

:: Make necessary directories
if not exist "%TEMP_DIR%" mkdir "%TEMP_DIR%"
if not exist "%RELEASE_DIR%" mkdir "%RELEASE_DIR%"

:: Copy files to make the final Zip structure
if exist "%DLL_PATH%" copy "%DLL_PATH%" "%TEMP_DIR%\" >nul
xcopy /S /I "%ASSETS_DIR%" "%TEMP_DIR%\Assets" >nul

tar -a -cf %ZIP_FILE% "%TEMP_DIR%" %FILES%

move "%ZIP_FILE%" "%RELEASE_DIR%\" >nul
rmdir /S /Q "%TEMP_DIR%"