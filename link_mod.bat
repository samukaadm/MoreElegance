@echo off
setlocal

set "MOD_DIR=%~dp0"
:: Remove trailing backslash if present
if "%MOD_DIR:~-1%"=="\" set "MOD_DIR=%MOD_DIR:~0,-1%"

echo ========================================================
echo    Elegant Melee (MoreElegance) - Vinculador ModSources
echo ========================================================
echo.

:: Check standard Documents paths
set "TARGET_DIR=%USERPROFILE%\Documents\My Games\Terraria\tModLoader\ModSources\MoreElegance"
set "ONEDRIVE_TARGET=%USERPROFILE%\OneDrive\Documentos\My Games\Terraria\tModLoader\ModSources\MoreElegance"

:: Determine parent directory
set "MODSOURCES_DIR=%USERPROFILE%\Documents\My Games\Terraria\tModLoader\ModSources"
if exist "%USERPROFILE%\OneDrive\Documentos\My Games\Terraria\tModLoader" (
    set "MODSOURCES_DIR=%USERPROFILE%\OneDrive\Documentos\My Games\Terraria\tModLoader\ModSources"
    set "TARGET_DIR=%ONEDRIVE_TARGET%"
)

if not exist "%MODSOURCES_DIR%" (
    echo Criando pasta ModSources em: "%MODSOURCES_DIR%"
    mkdir "%MODSOURCES_DIR%"
)

if exist "%TARGET_DIR%" (
    echo [OK] O link ou pasta ja existe em: "%TARGET_DIR%"
) else (
    echo Criando Juncao (Symlink de Diretorio):
    echo De: "%TARGET_DIR%"
    echo Para: "%MOD_DIR%"
    mklink /J "%TARGET_DIR%" "%MOD_DIR%"
    if errorlevel 1 (
        echo [ERRO] Falha ao criar juncao. Tente executar como Administrador.
        pause
        exit /b 1
    )
    echo [SUCESSO] Mod vinculado com sucesso ao tModLoader!
)

echo.
echo Agora basta abrir o tModLoader:
echo 1. Va em "Oficina" ^> "Desenvolver Mods".
echo 2. Clique em "Compilar e Recarregar" no MoreElegance.
echo.
pause
