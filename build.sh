#!/usr/bin/env bash


DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
source "$DIR/scripts/common.sh"

# True when we need to download dotnet as a script. Set when bootstrapping DNVM
manual_bootstrap=true

if [ "$manual_bootstrap" = true ]; then
    if [ ! -x "$DIR/.dotnet/dotnet" ]; then
        mkdir -p $DIR/.dotnet/
        curl -sSL https://dot.net/v1/dotnet-install.sh -o $DIR/.dotnet/dotnet-install.sh
        bash $DIR/.dotnet/dotnet-install.sh --install-dir $DIR/.dotnet/ --version 1.1.7
    fi

    export PATH="$DIR/.dotnet:$PATH"
fi

$DIR/scripts/install-tools.sh --no-dnvm
$DIR/scripts/pack.sh
