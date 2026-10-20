# Configuracao de seguranca

## Chave JWT

A API exige Jwt:Key durante a inicializacao. O valor precisa conter pelo menos 32 bytes UTF-8. Gere um valor aleatorio separado para cada ambiente e mantenha-o fora do repositorio, dos logs e das respostas HTTP.

No desenvolvimento local, use User Secrets da API:

PowerShell:
$jwtKey = [Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(32))
dotnet user-secrets set "Jwt:Key" $jwtKey --project src/Marketplace.Api
Remove-Variable jwtKey

Em ambientes hospedados, forneca o segredo por um secret manager ou pela variavel de ambiente Jwt__Key. Nao coloque o valor em appsettings.json versionado. A API falha rapidamente se a chave estiver ausente ou tiver menos de 32 bytes.

Jwt:Issuer e Jwt:Audience sao valores publicos de identificacao do emissor e do destinatario. Configure-os por ambiente quando diferirem dos valores versionados de desenvolvimento.

## CORS

As paginas Blazor Server chamam a API pelo servidor usando HttpClient, mas as operacoes de sessao de login/logout usam JavaScript no navegador e podem atravessar origens. Configure Cors:AllowedOrigins com as origens exatas do frontend em cada ambiente. A politica aceita credenciais apenas para essas origens. Nao use origem curinga; quando frontend e API compartilham a mesma origem, nao e necessario adicionar uma origem extra.

## Recuperacao de senha

Configure PasswordReset:PublicWebBaseUrl com a URL HTTPS publica do frontend. O link de redefinicao expira em uma hora.

O envio de email usa SMTP. Forneca Email:Smtp:Host, Email:Smtp:Port, Email:Smtp:EnableSsl, Email:Smtp:FromAddress, Email:Smtp:FromDisplayName, Email:Smtp:Username e Email:Smtp:Password por secrets ou variaveis de ambiente (Email__Smtp__...). Nunca versione credenciais. Sem SMTP configurado, a API continua retornando a resposta publica generica, mas registra a falha de entrega sem gravar login, email, token ou link.