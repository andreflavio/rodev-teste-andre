$outputFile = "esqueleto_atualizado.txt"
# Usar o diretório atual (onde o script está sendo executado)
$rootPath = Get-Location | Select-Object -ExpandProperty Path
$extensions = @(".cs", ".csproj", ".json", ".md", ".sln", ".gitignore")

# Verificar se o diretório existe
if (-not (Test-Path $rootPath)) {
    Write-Error "O diretório '$rootPath' não existe. Verifique o caminho e tente novamente."
    exit
}

# Escrever o cabeçalho com codificação UTF-8
"Esqueleto do Projeto: $rootPath" | Out-File $outputFile -Encoding UTF8
"--------------------------------------------------" | Out-File $outputFile -Append -Encoding UTF8

function List-Files($path, $indent = "") {
    try {
        $items = Get-ChildItem $path -ErrorAction Stop | Where-Object { 
            ($_.Extension -in $extensions) -or $_.PSIsContainer 
        } | Sort-Object Name
        foreach ($item in $items) {
            if ($item.PSIsContainer) {
                List-Files $item.FullName ($indent + "  ")
            } else {
                "$indent[Arquivo] $($item.Name)" | Out-File $outputFile -Append -Encoding UTF8
            }
        }
    } catch {
        Write-Error "Erro ao listar arquivos em '$path': $($_.Exception.Message)"
    }
}

List-Files $rootPath
Write-Host "Esqueleto gerado com sucesso em: $outputFile"