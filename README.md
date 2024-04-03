# PeopleInc Labs

## Descrição
A API REST que é responsável pela criação, atualização, deleção e consulta de Pessoas!
A api desenvolvida para o gerenciamento de pessoas composta por back-end, api que deve receber os dados da pessoa e adicionar, listar, atualizar e deletar

## Requisitos
Antes de executar o projeto, certifique-se de ter os seguintes requisitos instalados em sua máquina:
- .NET Core SDK
- DB Browser SQLite ou SQLite para visualizar o banco de dados

## Instalação
Siga os passos abaixo para instalar e executar o projeto:
- Clone este repositório do projeto
- Abra a api Ide Visual studio
- Em ferramentas abra o gerenciador de pacotes do NuGet e abra o console do gerenciador de pacotes
- Execute o comando dotnet build para compilar o projeto
- Vamos criar o banco de dados e as tabelas no SQLite através das migrations
- Execute o comando Add-Migration "Digite o nome para criar a migration"
- Execute o comando Update-Database para criar o banco de dados e as tabelas, também e utilizado para fazer as atualizações das tabelas

## Executar
- No Visual studio abasta clicar na seta de play para iniciar o servidor
- Se caso estiver usando outra Ide navegue até o diretório raiz do projeto
- Execute o comando dotnet run para iniciar o servidor
- Acesse o link http://localhost:5262/swagger/index.html para ter acesso aos os endpoints
