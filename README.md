# Ambiente de EXEMPLO para projetos ASPNET 6
Este projeto foi criado para motivos acadêmicos para minha aprendizagem pessoal
utilizando [C#](https://learn.microsoft.com/pt-br/dotnet/csharp/) e [Espnet 6](https://dotnet.microsoft.com/pt-br/apps/aspnet). 

## Screenshots
App view:
![App UI](/app.png)

## Development

### Setup

#### 1) Instalação de dependencias
<br>

##### Linguagem
È necessario que o [C#](https://learn.microsoft.com/pt-br/dotnet/csharp/) já esteja instalado em sua máquina

##### Serviços
``` sh
docker-compose up
```
Obs: Deixei uma aquivo de [Docker-Compose](https://docs.docker.com/compose/) para que a utilização deste 
projeto seja mais simples

### Extra

#### 1) QueryData
Criei uma forma simples de realizar pesquisas do dados nos endpoint´s paginados

Name | Description | Data
-----|-------------|------------------
Page               | Página atual | default: 1
PageSize           | Quantidades de itens por Página | default: 10
OrderBy            | Ordenação por atributo | default: ASC
OrderByDescending  | Tipo de ordenação | default: false => true=DESC, false=ASC 
FilterBy           | filtro de itens | default: empty => "Property" "Condition" "Value"

#### 3) FilterBy
Condition | Values | Exemple
-----|-------------|------------------
Equal              | "eq" or "Eq" or "EQ" | Name eq Ismael
NotEqual           | "ne" or "Ne" or "NE" | Name ne Ismael
GreaterThan        | "gt" or "Gt" or "GT" | Price gt 10
GreaterThanOrEqual | "ge" or "Ge" or "GE" | Price ge 10
LessThan           | "lt" or "Lt" or "LT" | Price lt 10
LessThanOrEqual    | "le" or "Le" or "LE" | Price le 10
Like               | "lk" or "Lk" or "LK" | Name lk ismael (only string columns)

#### 2) Helm
Deixei configurado o [helm](https://helm.sh/) para que sejá possivel trabalhar com [kubernetes](https://kubernetes.io/pt-br/).

## Contato
Desenvolvido por: [Ismael Alves](https://github.com/ismaelalvesgit) 🤓🤓🤓

* Email: [cearaismael1997@gmail.com](mailto:cearaismael1997@gmail.com) 
* Github: [github.com/ismaelalvesgit](https://github.com/ismaelalvesgit)
* Linkedin: [linkedin.com/in/ismael-alves-6945531a0/](https://www.linkedin.com/in/ismael-alves-6945531a0/)

### Customização de Configurações do projeto
Verifique [Configurações e Referencias](https://dotnet.microsoft.com/pt-br/apps/aspnet).
