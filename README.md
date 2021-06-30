# core-jwt-refresh-token-redis

Aplicação .Net Core 5.0 Web API desenvolvida com o objetivo de demonstrar uma rotina de login com geração de Token JWT e Refresh Token. Caso o Token esteja expirado é possível enviar o Refresh Token a fim de obter um novo token de autenticação. O Refresh Token é salvo através de cache baseado em Redis (Microsoft.Extensions.Caching.Redis)

# Este projeto contém:

- JWT (Bearer);
- Redis como mecanismo de cache; 
- Identity;

# Como executar:
- Clonar / baixar o repositório em seu workplace.
- Baixar o .Net Core SDK e o Visual Studio / Code mais recentes.
- Intalar o Redis local ou em container.
- Ajustar a conexão do Redis no projeto.

# Sobre
Este projeto foi desenvolvido por Anderson Hansen sob [MIT license](LICENSE).
