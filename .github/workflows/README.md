# CI/CD Workflows

Este diretório contém os workflows de CI/CD do projeto usando GitHub Actions.

## Workflow Disponível

### deploy.yml - Pipeline Completa

Pipeline unificada conforme especificação do desafio técnico.

**Triggers:**
- ✅ Push para branch `main`
- ✅ Pull Requests
- ✅ Manual (workflow_dispatch)

---

## Jobs da Pipeline

### 1️⃣ build-test-analyze

**Propósito:** Build, testes unitários e análise de código.

**Steps:**
1. **Checkout** - Clona o repositório
2. **Setup .NET 9.0** - Instala SDK
3. **Restore** - Restaura dependências NuGet
4. **Build** - Compila em modo Release
5. **Test** - Executa 99 testes unitários
6. **SonarCloud Scan** - Análise de qualidade e segurança

**Artifacts gerados:**
- Build outputs
- Test results
- Coverage reports

**Métricas SonarCloud:**
- ✅ Code Coverage
- ✅ Code Smells
- ✅ Bugs
- ✅ Vulnerabilities
- ✅ Security Hotspots
- ✅ Duplications

---

### 2️⃣ security-scan

**Propósito:** Scan de segurança com Trivy.

**Requires:** `build-test-analyze`

**Steps:**
1. **Checkout** - Clona o repositório
2. **Trivy Scan** - Escaneia vulnerabilidades no código
3. **Upload SARIF** - Envia resultados para GitHub Security

**Severidades detectadas:**
- 🔴 CRITICAL
- 🟠 HIGH
- 🟡 MEDIUM
- 🔵 LOW

**Resultados visíveis em:**
- Repository > Security > Code scanning alerts

---

### 3️⃣ build-and-push-docker

**Propósito:** Build e push de imagens Docker.

**Requires:** `build-test-analyze`, `security-scan`

**Steps:**
1. **Checkout** - Clona o repositório
2. **Docker Login** - Autentica no GitHub Container Registry (ghcr.io)
3. **Build Images** - Constrói 3 imagens Docker
4. **Push Images** - Envia para ghcr.io

**Imagens criadas:**
```
ghcr.io/seu-usuario/employee-cadastro:latest
ghcr.io/seu-usuario/employee-cadastro:sha-{commit}

ghcr.io/seu-usuario/employee-notificacoes:latest
ghcr.io/seu-usuario/employee-notificacoes:sha-{commit}

ghcr.io/seu-usuario/employee-ativacao:latest
ghcr.io/seu-usuario/employee-ativacao:sha-{commit}
```

**Tags:**
- `latest` - Sempre aponta para a última versão da main
- `sha-{commit}` - Tag específica do commit para rastreabilidade

---

## Configuração de Secrets

### Secrets Necessários

Acesse: **Repository > Settings > Secrets and variables > Actions**

#### 1. SONAR_TOKEN

**Como obter:**

```bash
# 1. Acesse SonarCloud
https://sonarcloud.io/

# 2. Login com GitHub

# 3. Import your organization
Organizations > Create an organization

# 4. Analyze new project
+ > Analyze new project > Selecione o repositório

# 5. Gerar token
Account (canto superior direito) > Security > Generate Token
- Name: github-actions
- Type: User Token
- Expires in: No expiration (ou conforme política)
```

**Adicionar no GitHub:**
```
Name: SONAR_TOKEN
Value: Cole o token gerado (ex: squ_a1b2c3d4e5f6...)
```

#### 2. GITHUB_TOKEN

**Não precisa configurar!** ✅

O `GITHUB_TOKEN` é gerado automaticamente pelo GitHub Actions.

**Apenas configure permissões:**
```
Repository > Settings > Actions > General > Workflow permissions
☑ Read and write permissions
```

---

## Configuração do SonarCloud

### Passo a Passo Completo

**1. Criar conta e organização:**
```
1. Acesse https://sonarcloud.io/
2. Click "Log in" > Login with GitHub
3. Authorize SonarCloud
4. Create Organization:
   - Choose GitHub organization
   - Plan: Free for public repos
```

**2. Importar projeto:**
```
1. Click "+" no canto superior direito
2. Analyze new project
3. Selecione: employee-management-microservice
4. Click "Set Up"
```

**3. Configurar Quality Gate:**
```
1. Project > Administration > Quality Gates
2. Use o "Sonar way" (padrão) ou crie customizado
3. Condições recomendadas:
   - Coverage on New Code > 80%
   - Duplicated Lines (%) on New Code < 3%
   - Maintainability Rating on New Code = A
   - Reliability Rating on New Code = A
   - Security Rating on New Code = A
```

**4. Gerar token:**
```
1. Account (seu avatar) > My Account > Security
2. Generate Token:
   - Name: github-actions-employee-management
   - Type: User Token
   - Expires in: No expiration
3. Click "Generate"
4. COPIE O TOKEN (só aparece uma vez!)
5. Adicione como secret SONAR_TOKEN no GitHub
```

**5. Configurar projeto no GitHub Actions:**

O workflow já está configurado! Apenas certifique-se que:
```yaml
SONAR_ORGANIZATION: "sua-org"  # Nome da sua org no SonarCloud
SONAR_PROJECT_KEY: "seu-usuario_employee-management-microservice"
```

---

## Visualizar Resultados

### GitHub Actions

**Acompanhar execuções:**
```
Repository > Actions > Deploy workflow
```

**Visualizar detalhes:**
- Click em uma execução
- Veja logs de cada job
- Download de artifacts

### SonarCloud Dashboard

**Acessar análises:**
```
https://sonarcloud.io/dashboard?id=seu-usuario_employee-management-microservice
```

**Métricas disponíveis:**
- 📊 Overview (bugs, vulnerabilities, code smells)
- 📈 Measures (métricas detalhadas)
- 🔍 Issues (problemas encontrados)
- 🛡️ Security Hotspots
- 📉 Coverage (cobertura de testes)
- 🔄 Activity (histórico)

### GitHub Security

**Vulnerabilidades Trivy:**
```
Repository > Security > Code scanning
```

Alerts são criados automaticamente para cada vulnerabilidade encontrada.

---

## Status Badges

Adicione ao README.md para mostrar status da pipeline:

```markdown
<!-- Build Status -->
[![Deploy Pipeline](https://github.com/seu-usuario/employee-management-microservice/actions/workflows/deploy.yml/badge.svg)](https://github.com/seu-usuario/employee-management-microservice/actions/workflows/deploy.yml)

<!-- SonarCloud Quality Gate -->
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=seu-usuario_employee-management-microservice&metric=alert_status)](https://sonarcloud.io/dashboard?id=seu-usuario_employee-management-microservice)

<!-- SonarCloud Coverage -->
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=seu-usuario_employee-management-microservice&metric=coverage)](https://sonarcloud.io/dashboard?id=seu-usuario_employee-management-microservice)

<!-- Reliability -->
[![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=seu-usuario_employee-management-microservice&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=seu-usuario_employee-management-microservice)

<!-- Security -->
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=seu-usuario_employee-management-microservice&metric=security_rating)](https://sonarcloud.io/dashboard?id=seu-usuario_employee-management-microservice)

<!-- Maintainability -->
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=seu-usuario_employee-management-microservice&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=seu-usuario_employee-management-microservice)
```

**Substitua:**
- `seu-usuario` pelo seu username do GitHub
- `employee-management-microservice` pelo nome do repositório

---

## Fluxo da Pipeline

```
┌──────────────┐
│   Trigger    │  Push to main, PR, ou Manual
└──────┬───────┘
       │
       ▼
┌──────────────────────────┐
│  build-test-analyze      │
│  • Checkout              │
│  • Setup .NET 9.0        │
│  • Restore dependencies  │
│  • Build (Release)       │
│  • Run 99 tests          │
│  • SonarCloud analysis   │
└──────┬───────────────────┘
       │
       ├─────────────┬────────────────┐
       │             │                │
       ▼             ▼                ▼
┌─────────────┐ ┌──────────┐ ┌─────────────────┐
│ security-   │ │ build-   │ │                 │
│ scan        │ │ and-push │ │  Wait for       │
│ • Trivy     │ │ -docker  │ │  all jobs       │
│ • Upload    │ │ • Login  │ │                 │
│   SARIF     │ │ • Build  │ │                 │
└─────────────┘ │ • Push   │ │                 │
                └──────────┘ └─────────────────┘
                     │
                     ▼
              ┌─────────────┐
              │   SUCCESS   │ ✅
              │  Images on  │
              │   ghcr.io   │
              └─────────────┘
```

---

## Troubleshooting

### ❌ Erro: "SonarCloud analysis failed"

**Causa possível:**
- Token inválido ou expirado
- Organização/projeto não configurados
- Permissões insuficientes

**Solução:**
```bash
1. Verificar se SONAR_TOKEN está configurado corretamente
2. Gerar novo token no SonarCloud
3. Verificar se organization e project key estão corretos no workflow
4. Verificar logs detalhados no GitHub Actions
```

---

### ❌ Erro: "Docker push failed: denied"

**Causa possível:**
- Permissões de workflow insuficientes
- GITHUB_TOKEN sem acesso a packages

**Solução:**
```bash
1. Repository > Settings > Actions > General
2. Workflow permissions > Read and write permissions
3. ☑ Allow GitHub Actions to create and approve pull requests
4. Save
5. Re-run workflow
```

---

### ❌ Erro: "Tests failed"

**Causa possível:**
- Testes falhando localmente
- Dependências faltando
- Configuração de ambiente

**Solução:**
```bash
# Rodar testes localmente primeiro
dotnet test --verbosity detailed

# Verificar falhas específicas
dotnet test --logger "console;verbosity=detailed"

# Corrigir testes
# Commit e push novamente
```

---

### ❌ Erro: "Trivy found vulnerabilities"

**Causa possível:**
- Vulnerabilidades CRITICAL ou HIGH detectadas

**Solução:**
```bash
1. Acessar: Repository > Security > Code scanning
2. Ver detalhes das vulnerabilidades
3. Atualizar pacotes vulneráveis:
   - dotnet list package --vulnerable
   - dotnet add package NomePackage --version X.Y.Z
4. Ou suprimir falsos positivos (com justificativa)
```

---

### ⚠️ Warning: "Coverage decreased"

**Causa possível:**
- Código novo sem testes
- Testes removidos

**Solução:**
```bash
# Verificar cobertura localmente
dotnet test /p:CollectCoverage=true

# Adicionar testes para novo código
# Manter cobertura > 80%
```

---

## Otimizações Implementadas

### ✅ Cache de Dependências
```yaml
- uses: actions/setup-dotnet@v3
  with:
    dotnet-version: '9.0.x'
    cache: true  # ← Cache automático de pacotes NuGet
```

**Benefício:** Build 2-3x mais rápido após primeira execução.

---

### ✅ Jobs Paralelos
```yaml
security-scan:
  needs: build-test-analyze  # ← Executa após build

build-and-push-docker:
  needs:
    - build-test-analyze     # ← Executa após ambos
    - security-scan
```

**Benefício:** Jobs independentes rodam simultaneamente.

---

### ✅ Fail-fast
```yaml
if: github.event_name != 'pull_request'  # ← Skip em PRs
```

**Benefício:** Docker build só roda em push para main (economiza tempo/recursos).

---

## Boas Práticas

### ✅ Commits Semânticos

Use conventional commits para melhor rastreabilidade:

```bash
feat: adiciona endpoint de relatórios
fix: corrige validação de telefone
docs: atualiza README com exemplos
test: adiciona testes para EmployeeService
chore: atualiza dependências
refactor: simplifica lógica de ativação
```

---

### ✅ Proteção de Branches

Configure branch protection rules:

```
Repository > Settings > Branches > Add rule

Branch name pattern: main

☑ Require status checks to pass before merging
  ☑ build-test-analyze
  ☑ security-scan

☑ Require pull request reviews before merging
  Required approvals: 1

☑ Do not allow bypassing the above settings
```

---

### ✅ Monitoramento Contínuo

**Acompanhe regularmente:**
- ✅ GitHub Actions: execuções e falhas
- ✅ SonarCloud: quality gate e métricas
- ✅ GitHub Security: vulnerabilidades detectadas
- ✅ Coverage: mantenha > 80%

---

## Referências

**GitHub Actions:**
- [Documentação Oficial](https://docs.github.com/en/actions)
- [Workflow Syntax](https://docs.github.com/en/actions/reference/workflow-syntax-for-github-actions)

**SonarCloud:**
- [Documentation](https://docs.sonarcloud.io/)
- [Quality Gates](https://docs.sonarcloud.io/improving/quality-gates/)
- [.NET Analysis](https://docs.sonarcloud.io/advanced-setup/languages/csharp-vb-net/)

**Trivy:**
- [Documentation](https://aquasecurity.github.io/trivy/)
- [GitHub Action](https://github.com/aquasecurity/trivy-action)

**Container Registry:**
- [GitHub Packages](https://docs.github.com/en/packages)
- [Working with GHCR](https://docs.github.com/en/packages/working-with-a-github-packages-registry/working-with-the-container-registry)

---

**Última atualização:** 2025-10-24
