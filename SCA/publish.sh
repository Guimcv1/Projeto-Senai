#!/bin/bash
# Script para publicar a aplicação SCA corretamente

echo "Iniciando publicação do projeto SCA..."

# O comando precisa especificar o arquivo SCA.csproj
# Dessa forma, ele não tenta aplicar as configurações de 'SingleFile' aos projetos de biblioteca (SCA.Core) e de testes (SCA.Tests), o que causava o erro NETSDK1099 e NETSDK1098.

dotnet publish SCA.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true

if [ $? -eq 0 ]; then
    echo -e "\n Publicação concluída com sucesso!"
    echo "O executável está em: bin/Release/net10.0/win-x64/publish/SCA.exe"
else
    echo -e "\n Erro durante a publicação."
fi
