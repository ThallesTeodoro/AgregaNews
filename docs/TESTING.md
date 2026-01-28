# Guia de Testes

Este guia contém instruções detalhadas sobre como testar a aplicação AgregaNews.

## 1. Verificar se os serviços estão rodando

Primeiro, verifique se todos os serviços estão saudáveis:

```bash
# Verificar CollectNews API
curl http://localhost:5100/health

# Verificar AnalyzeNews API
curl http://localhost:5200/health

# Verificar Log Service API
curl http://localhost:5300/health

# Verificar Gateway API
curl http://localhost:5000/health
```

Ou abra no navegador:
- http://localhost:5100/health
- http://localhost:5200/health
- http://localhost:5300/health
- http://localhost:5000/health

## 2. Testar a coleta de notícias

**Via Gateway (recomendado):**

```bash
# Coletar notícias do Brasil (padrão)
curl "http://localhost:5000/collect-news-api/collect-news?country=br&pageSize=5"

# Coletar notícias de tecnologia do Brasil
curl "http://localhost:5000/collect-news-api/collect-news?country=br&category=technology&pageSize=5"

# Coletar notícias de esportes dos EUA
curl "http://localhost:5000/collect-news-api/collect-news?country=us&category=sports&pageSize=10"
```

**Diretamente no serviço:**

```bash
curl "http://localhost:5100/collect-news?country=br&pageSize=5"
```

**Parâmetros disponíveis:**

- `country` (obrigatório): Código ISO do país (ex: `br`, `us`, `gb`)
- `category` (opcional): Categoria da notícia
  - `business`, `entertainment`, `general`, `health`, `science`, `sports`, `technology`
- `pageSize` (opcional): Quantidade de notícias por página (1-100, padrão: 10)
- `page` (opcional): Número da página (padrão: 1)

## 3. Verificar notícias analisadas

Após coletar notícias, aguarde alguns segundos para que o AnalyzeNews processe e categorize as notícias:

```bash
# Ver notícias analisadas (últimas 10)
curl "http://localhost:5000/analyze-news-api/analyze-news?size=10"

# Ver mais notícias
curl "http://localhost:5000/analyze-news-api/analyze-news?size=20"
```

**Parâmetros:**

- `size` (opcional): Quantidade de notícias analisadas (1-100, padrão: 10)

## 4. Verificar logs

```bash
# Ver logs com paginação
curl "http://localhost:5000/log-api/logs?page=1&pageSize=10"
```

**Parâmetros:**

- `page` (opcional): Número da página (padrão: 1)
- `pageSize` (opcional): Itens por página (padrão: 10)

## 5. Usar Swagger UI (mais fácil)

A maneira mais fácil de testar é usando a interface Swagger:

1. **CollectNews API**: http://localhost:5100
2. **AnalyzeNews API**: http://localhost:5200
3. **Log Service API**: http://localhost:5300
4. **Gateway API**: http://localhost:5000

Na interface Swagger, você pode:

- Ver todos os endpoints disponíveis
- Testar diretamente na interface
- Ver exemplos de requisição e resposta

## 6. Testar o fluxo completo

**Passo a passo para testar todo o fluxo:**

```bash
# 1. Coletar notícias (dispara o processo)
curl "http://localhost:5000/collect-news-api/collect-news?country=br&category=technology&pageSize=3"

# 2. Aguardar 5-10 segundos para processamento assíncrono

# 3. Verificar notícias analisadas e categorizadas
curl "http://localhost:5000/analyze-news-api/analyze-news?size=10"

# 4. Verificar logs gerados
curl "http://localhost:5000/log-api/logs?page=1&pageSize=5"
```

## 7. Monitorar RabbitMQ

Acesse o painel de gerenciamento do RabbitMQ:

- URL: http://localhost:15672
- Usuário: `mq_user`
- Senha: `secret`

No painel você pode:

- Ver filas de mensagens
- Monitorar consumo de eventos
- Ver estatísticas de mensagens

## 8. Verificar logs dos serviços

**No Docker:**

```bash
# Logs do CollectNews
docker logs AgregaNews.CollectNews.Api

# Logs do AnalyzeNews
docker logs AgregaNews.AnalyzeNews.Api

# Logs do Log Service
docker logs AgregaNews.Log.Api

# Logs do Gateway
docker logs AgregaNews.GatwayApi

# Ver todos os logs em tempo real
docker-compose logs -f
```

## 9. Verificar dados no MongoDB

Você pode verificar os dados salvos nos bancos MongoDB:

```bash
# Conectar ao MongoDB do CollectNews
docker exec -it AgregaNews.CollectNews.Database mongosh -u dbusername -p secret

# Dentro do mongosh:
use CollectNewsDb
db.News.find().pretty()

# Conectar ao MongoDB do AnalyzeNews
docker exec -it AgregaNews.AnalyzeNews.Database mongosh -u dbusername -p secret

# Dentro do mongosh:
use AnalyzeNewsDb
db.AnalyzedNews.find().pretty()

# Conectar ao MongoDB do Log Service
docker exec -it AgregaNews.Log.Database mongosh -u dbusername -p secret

# Dentro do mongosh:
use LogDb
db.Log.find().pretty()
```

## 10. Testes unitários

Execute os testes unitários do projeto:

```bash
dotnet test
```

## Exemplos de Resposta

### Coleta de notícias bem-sucedida:

```json
{
  "statusCode": 200,
  "message": null,
  "errors": null,
  "data": [
    {
      "id": "guid-aqui",
      "author": "Nome do Autor",
      "title": "Título da Notícia",
      "description": "Descrição da notícia...",
      "url": "https://exemplo.com/noticia",
      "urlToImage": "https://exemplo.com/imagem.jpg",
      "publishedAt": "2024-01-15T10:00:00Z",
      "content": "Conteúdo completo da notícia..."
    }
  ]
}
```

### Notícias analisadas:

```json
{
  "statusCode": 200,
  "data": [
    {
      "id": "guid-aqui",
      "title": "Título da Notícia",
      "category": "Tecnologia",
      "author": "Nome do Autor",
      "url": "https://exemplo.com/noticia",
      "publishedAt": "2024-01-15T10:00:00Z",
      "createdAt": "2024-01-15T10:05:00Z"
    }
  ]
}
```

## Troubleshooting

### Problema: "ApiKey não configurado"

- Verifique se você adicionou as chaves de API nos arquivos `appsettings.json`

### Problema: Serviços não respondem

- Verifique se todos os containers estão rodando: `docker-compose ps`
- Verifique os logs: `docker-compose logs`

### Problema: Notícias não são categorizadas

- Verifique se a chave do OpenAI está configurada corretamente
- Verifique os logs do AnalyzeNews para erros
- Verifique se o RabbitMQ está recebendo eventos

### Problema: Health check falha

- Verifique se o MongoDB e RabbitMQ estão rodando
- Verifique as strings de conexão nos `appsettings.json`
