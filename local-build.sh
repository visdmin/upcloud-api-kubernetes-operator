#!/bin/bash
# https://github.com/buehler/dotnet-operator-sdk/issues/586

if [[ "$OSTYPE" == "linux-gnu"* ]];
then
    export CFSSL_EXECUTABLES_PATH="$(pwd)/cfssl/linux"
elif [[ "$OSTYPE" == "cygwin" ]] ||  [[ "$OSTYPE" == "msys" ]] || [[ "$OSTYPE" == "win32" ]];
then
    export CFSSL_EXECUTABLES_PATH="$(pwd)/cfssl/win"
else
    echo "Shell running in uknown operating system ${OSTYPE}";
    exit 1
fi

dotnet build
