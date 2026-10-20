# Runbook de preparacao e publicacao

Este documento descreve a preparacao de uma publicacao; o repositorio nao realiza deploy automatico. Confirme ambiente, dominio, backup e janela de manutencao com o responsavel antes de publicar.

## Requisitos

- Runtime ASP.NET Core .NET 10 para API e Blazor Web.
- SQL Server acessivel pela API, com usuario de banco de privilegio minimo para a aplicacao.
- HTTPS no proxy/reverse proxy e nos dominios publicos. Configure o proxy para encaminhar corretamente host e protocolo.
- Armazenamento persistente e backup para src/Marketplace.Api/wwwroot/uploads (produtos e carrossel).
- Persistencia compartilhada dos dados de ASP.NET Data Protection, pois as chaves protegem cookies e tokens de autenticacao.

## Configuracao obrigatoria

Defina as chaves no secret store do ambiente ou em variaveis de ambiente do processo. O provedor de configuracao do .NET converte dois underscores em dois pontos.

| Nome | Uso |
| --- | --- |
| ConnectionStrings__Marketplace | Conexao SQL Server da API. |
| Jwt__Key | Chave aleatoria com pelo menos 32 bytes UTF-8; gere uma chave exclusiva por ambiente. |
| Jwt__Issuer e Jwt__Audience | Issuer e audience dos tokens; configure valores proprios do ambiente. |
| Cors__AllowedOrigins__0 (e indices adicionais) | Origem HTTPS exata do frontend, sem caminho; a API rejeita origens fora da lista. |
| PasswordReset__PublicWebBaseUrl | URL publica HTTPS do Blazor usada nos links de redefinicao. |
| Email__Smtp__Host, Email__Smtp__Port, Email__Smtp__EnableSsl | Servidor de email transacional e transporte TLS. |
| Email__Smtp__FromAddress e Email__Smtp__FromDisplayName | Remetente configurado e verificado no provedor. |
| Email__Smtp__Username e Email__Smtp__Password | Credenciais SMTP mantidas somente no secret store. |
| DataProtection__KeyDirectory | Diretorio duravel, restrito e compartilhado entre replicas da API. |
| Authentication__CookieDomain | Opcional; dominio comum quando Web/API usam subdominios e compartilham cookie. |
| ApiBaseUrl | URL publica da API consumida pelo host Blazor. |

Gere Jwt__Key usando um gerador criptograficamente seguro e guarde o valor fora do repositorio. Nunca o imprima em logs ou pipelines. Mantenha a mesma configuracao de Data Protection nas replicas que precisam validar os mesmos cookies.

## Preparar uma publicacao

1. Revise o commit e confirme que o workflow CI passou, inclusive os testes Playwright.
2. Publique os dois projetos para pastas/artifacts separados:

   dotnet publish src/Marketplace.Api/Marketplace.Api.csproj -c Release -o artifacts/api

   dotnet publish src/Marketplace.Web/Marketplace.Web.csproj -c Release -o artifacts/web

3. Configure secrets, conexao SQL, allowlist CORS, URLs publicas e volumes persistentes no ambiente alvo. Configure TLS antes de receber trafego.
4. Faça backup validado do banco e de uploads; registre o commit atualmente implantado.
5. Aplique migrations com a mesma versao do codigo usando dotnet ef database update --project src/Marketplace.Api/Marketplace.Api.csproj quando o projeto tiver migrations; se o ambiente ainda usar o esquema legado sem migrations, siga o procedimento de banco aprovado e valide o esquema antes de subir a API. Nunca habilite SeedDatabase em producao.
6. Inicie a API e o Blazor, confirme o health check GET /api/health, login, busca de catalogo, acesso aos uploads e envio/recebimento de email de recuperacao.
7. Confirme logs de startup e erros, depois libere trafego no proxy.

## Rollback

1. Interrompa a liberacao de trafego e volte API e Web juntos para os artifacts do commit anterior.
2. Verifique /api/health, autenticacao, catalogo e imagens.
3. Restaure o banco somente se uma migration incompatível tiver alterado dados/esquema; restaure junto o backup de uploads correspondente. Nao reverta dados automaticamente sem avaliar gravacoes feitas apos o deploy.
4. Registre o commit, migration e motivo do rollback; corrija o problema e repita CI antes de nova publicacao.

## Limites

Nenhum provedor ou processo de deploy foi escolhido neste projeto. Este runbook nao executa deploy, migration ou backup em infraestrutura real.
