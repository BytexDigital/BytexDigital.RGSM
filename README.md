# RGSM
RGSM (Remote GameServer Management) is a webpanel for game server administration. The goal of RGSM is to provide a free and feature rich management experience for common administration tasks.

## Information for server admins
:warning: **This panel is not yet usable by end consumers.**

## Information for developers

### Project Structure
#### BytexDigital.RGSM.Node.*
Node/Worker project that runs on machines also running the gameservers.

#### BytexDigital.RGSM.Panel.Server.*
ASP.NET Core Web API (with minimal login UI) that holds logic regarding user accounts and groups.
The panel is the authentication provider across all projects by issuing JWTs.

#### BytexDigital.RGSM.Panel.Client.*
Blazor WASM project that acts as the frontend to both the panel and nodes. Once a JWT is received from the panel, the client directly calls API endpoints on the nodes of the network.

### Requirements
- Nuget feed `https://nuget.bytex.digital/v3/index.json`