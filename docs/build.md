# Build Guide

## Build EDCHost

Run the following command to build the program:

```shell
dotnet publish src/EdcHost/ -o bin -p:PublishSingleFile=true -r win-x64 --sc false
```

Checkout artifacts in `bin/` directory.

## Build API Reference

The API are defined following AsyncAPI specifications. To build the API references, AsyncAPI CLI tool is required.

```shell
ag docs/api/asyncapi.yaml @asyncapi/html-template -o site/api/
```
