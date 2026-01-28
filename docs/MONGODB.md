# Validação e Uso do MongoDB

Este guia mostra como validar e trabalhar com as instâncias MongoDB do AgregaNews.

## Instâncias MongoDB

O sistema possui 3 instâncias MongoDB, cada uma para um microserviço:

1. **CollectNews Database** - Porta `27017`
2. **AnalyzeNews Database** - Porta `27018`
3. **Log Database** - Porta `27019`

## Credenciais

- **Usuário:** `dbusername`
- **Senha:** `secret`
- **Database de autenticação:** `admin`

## Validação Rápida

### 1. Verificar se os containers estão rodando

```bash
docker ps --filter "name=Database"
```

### 2. Testar conexão (Ping)

**CollectNews:**

```bash
docker exec AgregaNews.CollectNews.Database mongosh --eval "db.adminCommand('ping')" -u dbusername -p secret --authenticationDatabase admin
```

**AnalyzeNews:**

```bash
docker exec AgregaNews.AnalyzeNews.Database mongosh --eval "db.adminCommand('ping')" -u dbusername -p secret --authenticationDatabase admin
```

**Log Service:**

```bash
docker exec AgregaNews.Log.Database mongosh --eval "db.adminCommand('ping')" -u dbusername -p secret --authenticationDatabase admin
```

### 3. Listar bancos de dados

**CollectNews:**

```bash
docker exec AgregaNews.CollectNews.Database mongosh -u dbusername -p secret --authenticationDatabase admin --eval "show dbs"
```

## Conectar via Shell Interativo

### Windows (PowerShell)

Para conectar interativamente, você pode usar:

```bash
docker exec -it AgregaNews.CollectNews.Database mongosh -u dbusername -p secret --authenticationDatabase admin
```

Dentro do shell do MongoDB:

```javascript
// Trocar para o banco do serviço
use CollectNewsDb

// Listar coleções
show collections

// Contar documentos
db.News.countDocuments()

// Ver alguns documentos
db.News.find().limit(5).pretty()

// Ver um documento específico
db.News.findOne()
```

### Linux/Mac

```bash
docker exec -it AgregaNews.CollectNews.Database mongosh -u dbusername -p secret --authenticationDatabase admin
```

## Comandos Úteis por Serviço

### CollectNews Database

```bash
# Conectar ao banco
docker exec AgregaNews.CollectNews.Database mongosh -u dbusername -p secret --authenticationDatabase admin

# Dentro do mongosh:
use CollectNewsDb
db.News.find().limit(5).pretty()
db.News.countDocuments()
db.News.find({title: /tecnologia/i}).pretty()
```

### AnalyzeNews Database

```bash
# Conectar ao banco
docker exec AgregaNews.AnalyzeNews.Database mongosh -u dbusername -p secret --authenticationDatabase admin

# Dentro do mongosh:
use AnalyzeNewsDb
db.AnalyzedNews.find().limit(5).pretty()
db.AnalyzedNews.countDocuments()
db.AnalyzedNews.find({category: "Tecnologia"}).pretty()
```

### Log Database

```bash
# Conectar ao banco
docker exec AgregaNews.Log.Database mongosh -u dbusername -p secret --authenticationDatabase admin

# Dentro do mongosh:
use LogDb
db.Log.find().limit(10).pretty()
db.Log.countDocuments()
db.Log.find({severity: "Error"}).pretty()
```

## Conectar via Cliente MongoDB (MongoDB Compass)

Você também pode usar o MongoDB Compass para conectar visualmente:

### CollectNews

- **Connection String:** `mongodb://dbusername:secret@localhost:27017/CollectNewsDb?authSource=admin`

### AnalyzeNews

- **Connection String:** `mongodb://dbusername:secret@localhost:27018/AnalyzeNewsDb?authSource=admin`

### Log Service

- **Connection String:** `mongodb://dbusername:secret@localhost:27019/LogDb?authSource=admin`

## Validação Automatizada

### Verificar status de todos os bancos

```bash
# CollectNews
docker exec AgregaNews.CollectNews.Database mongosh -u dbusername -p secret --authenticationDatabase admin --eval "use CollectNewsDb; db.stats()"

# AnalyzeNews
docker exec AgregaNews.AnalyzeNews.Database mongosh -u dbusername -p secret --authenticationDatabase admin --eval "use AnalyzeNewsDb; db.stats()"

# Log
docker exec AgregaNews.Log.Database mongosh -u dbusername -p secret --authenticationDatabase admin --eval "use LogDb; db.stats()"
```

### Contar documentos em todas as coleções

```bash
# CollectNews
docker exec AgregaNews.CollectNews.Database mongosh -u dbusername -p secret --authenticationDatabase admin --eval "use CollectNewsDb; db.News.countDocuments()"

# AnalyzeNews
docker exec AgregaNews.AnalyzeNews.Database mongosh -u dbusername -p secret --authenticationDatabase admin --eval "use AnalyzeNewsDb; db.AnalyzedNews.countDocuments()"

# Log
docker exec AgregaNews.Log.Database mongosh -u dbusername -p secret --authenticationDatabase admin --eval "use LogDb; db.Log.countDocuments()"
```

## Troubleshooting

### Problema: "Authentication failed"

Verifique se as credenciais estão corretas:

- Usuário: `dbusername`
- Senha: `secret`
- Auth Database: `admin`

### Problema: "Cannot connect"

Verifique se o container está rodando:

```bash
docker ps --filter "name=Database"
```

Se não estiver rodando:

```bash
docker-compose up -d agreganews.collectnews.database
```

### Problema: "Database not found"

Os bancos são criados automaticamente quando os serviços fazem a primeira escrita. Se não existirem, execute uma operação de coleta de notícias primeiro.

## Limpar Dados (Desenvolvimento)

**Cuidado:** Isso apaga todos os dados!

```bash
# CollectNews
docker exec AgregaNews.CollectNews.Database mongosh -u dbusername -p secret --authenticationDatabase admin --eval "use CollectNewsDb; db.News.deleteMany({})"

# AnalyzeNews
docker exec AgregaNews.AnalyzeNews.Database mongosh -u dbusername -p secret --authenticationDatabase admin --eval "use AnalyzeNewsDb; db.AnalyzedNews.deleteMany({})"

# Log
docker exec AgregaNews.Log.Database mongosh -u dbusername -p secret --authenticationDatabase admin --eval "use LogDb; db.Log.deleteMany({})"
```
