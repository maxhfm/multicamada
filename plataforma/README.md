# Minha Aplicação — 3 Camadas no Kubernetes

```
┌─────────────────────────────────────────────────┐
│               Ingress (sslip.io)                │
└──────────────────────┬──────────────────────────┘
                       │ HTTP
┌──────────────────────▼──────────────────────────┐
│          Frontend  (nginx + HTML)               │
│              frontend-service:80                │
└──────────────────────┬──────────────────────────┘
                       │ proxy /api → :8080
┌──────────────────────▼──────────────────────────┐
│           Backend  (C# .NET 8)                  │
│              backend-service:8080               │
└──────────────────────┬──────────────────────────┘
                       │ TCP 3306
┌──────────────────────▼──────────────────────────┐
│            MySQL 8  (+ PVC 5 Gi)                │
│              mysql-service:3306                 │
└─────────────────────────────────────────────────┘
```

## Estrutura de arquivos

```
app/
├── frontend/
│   ├── index.html       # Interface com campo + botão
│   ├── nginx.conf       # Proxy /api → backend-service
│   └── Dockerfile
├── backend/
│   ├── Program.cs       # API mínima .NET 8
│   ├── Backend.csproj
│   ├── appsettings.json
│   └── Dockerfile
└── k8s/
    ├── 00-namespace.yaml
    ├── 01-secrets-configmap.yaml
    ├── 02-mysql.yaml
    ├── 03-backend.yaml
    ├── 04-frontend.yaml
    └── 05-ingress.yaml
```

## Pré-requisitos

- Cluster Kubernetes (k3s, EKS, GKE, etc.)
- `kubectl` configurado
- Registry de imagens acessível (Docker Hub, ECR, etc.)
- Ingress NGINX Controller instalado

## 1. Instalar o Ingress Controller (se necessário)

```bash
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.10.1/deploy/static/provider/cloud/deploy.yaml
```

## 2. Build e push das imagens

```bash
# Frontend
docker build -t sua-registry/frontend:latest ./frontend
docker push sua-registry/frontend:latest

# Backend
docker build -t sua-registry/backend:latest ./backend
docker push sua-registry/backend:latest
```

## 3. Ajustar credenciais do banco

Edite `k8s/01-secrets-configmap.yaml` com valores em base64:

```bash
echo -n 'suaSenhaRoot' | base64
echo -n 'suaSenhaApp'  | base64
```

## 4. Ajustar imagens nos deployments

Edite `k8s/03-backend.yaml` e `k8s/04-frontend.yaml`:
```yaml
image: sua-registry/backend:latest
image: sua-registry/frontend:latest
```

## 5. Ajustar o domínio no Ingress

Descubra o IP do seu cluster:
```bash
kubectl get svc -n ingress-nginx ingress-nginx-controller
```

Edite `k8s/05-ingress.yaml`:
```yaml
- host: 203-0-113-42-MINHAAPLICACAO.sslip.io
#         ^^^^^^^^^^^^^^ seu IP com traços
```

## 6. Deploy

```bash
kubectl apply -f k8s/
```

## 7. Verificar

```bash
kubectl get all -n minha-aplicacao
kubectl get ingress -n minha-aplicacao
```

## Endpoints da API

| Método | Path        | Descrição                  |
|--------|-------------|----------------------------|
| POST   | /api/entrada | Grava mensagem no banco    |
| GET    | /api/entrada | Lista todas as mensagens   |
| GET    | /api/health  | Health-check               |
