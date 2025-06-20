@echo off
echo === Pokrecem SVG optimizaciju za FactoryTalk Optix ===

:: Provjera da su folderi tu
IF NOT EXIST "svg_input" (
    echo [GRESKA] Folder "svg_input" ne postoji.
    pause
    exit /b
)

IF NOT EXIST "svg_output" (
    echo [INFO] Kreiram folder "svg_output"...
    mkdir svg_output
)

:: Pokreni SVGO za sve SVG datoteke
svgo -f ./svg_input -o ./svg_output --config .svgo.config.js

echo.
echo === Gotovo! SVG-ovi su optimizirani u folderu "svg_output". ===
pause
