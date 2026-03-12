# Semantic Kernel Notes

Este documento contém **anotações técnicas aprofundadas sobre o Semantic Kernel**, focadas em como ele funciona internamente e como projetar aplicações reais com ele.

O objetivo é entender **a arquitetura do framework**, não apenas executar exemplos.

---

# 1. O que é o Semantic Kernel

O **Semantic Kernel (SK)** é um framework de **orquestração de IA** criado para integrar **LLMs com código tradicional**.

Ele permite que modelos de linguagem:

* usem funções do seu código
* acessem dados
* utilizem memória
* executem tarefas complexas

Arquitetura conceitual:

```
Application
      ↓
Semantic Kernel
      ↓
LLM Services
Plugins / Tools
Memory
Agents
```

O Kernel funciona como **um runtime para aplicações de IA**.

---

# 2. O Kernel

O **Kernel** é o componente central do framework.

Ele é responsável por:

* registrar serviços de IA
* registrar plugins
* executar funções
* gerenciar contexto
* coordenar chamadas ao LLM

Exemplo conceitual:

```
Kernel
 ├── AI Services
 ├── Plugins
 ├── Memory
 └── Execution Pipeline
```

No código C#, o Kernel geralmente é inicializado assim:

```
var builder = Kernel.CreateBuilder();
var kernel = builder.Build();
```

Depois disso você adiciona:

* modelos
* plugins
* serviços

---

# 3. AI Services

AI Services são os **provedores de modelos** usados pelo Kernel.

Exemplos:

* OpenAI
* Azure OpenAI
* modelos locais
* provedores externos

Esses serviços geralmente fornecem:

```
Chat Completion
Text Completion
Embeddings
```

Fluxo:

```
Kernel
   ↓
AI Service
   ↓
LLM
```

---

# 4. Plugins

Plugins são **coleções de funções que o LLM pode utilizar**.

Eles representam **capacidades do sistema**.

Exemplo conceitual:

```
PedidosPlugin
 ├── ObterPedidosHoje
 ├── ListarPedidosCliente
 └── CalcularFaturamento
```

Plugins conectam o LLM com:

* bancos de dados
* APIs
* lógica de negócio

---

# 5. Kernel Functions

Cada função exposta ao Kernel é chamada de **Kernel Function**.

Essas funções podem ser:

### 1. Funções nativas

Escritas em C#.

Exemplo conceitual:

```
ObterPedidosHoje()
EnviarEmail()
BuscarCliente()
```

### 2. Funções semânticas

Baseadas em **prompts que usam o LLM**.

Exemplo:

```
ResumirTexto()
TraduzirConteudo()
ExplicarRelatorio()
```

---

# 6. Prompt Templates

Prompt Templates são **prompts estruturados que recebem variáveis**.

Eles permitem reutilizar prompts de forma organizada.

Exemplo:

```
Resuma o seguinte texto:

{{$input}}
```

O valor de `input` é preenchido dinamicamente.

Isso transforma prompts em **componentes reutilizáveis do sistema**.

---

# 7. Contexto de Execução

Quando o Kernel executa uma função, ele cria um **contexto de execução**.

Esse contexto contém:

* variáveis
* histórico
* resultados intermediários

Fluxo:

```
Input
↓
Execution Context
↓
Function
↓
Output
```

Esse mecanismo permite **encadear funções**.

---

# 8. Pipeline de Execução

O Semantic Kernel executa tarefas em **pipelines**.

Exemplo de fluxo:

```
User input
      ↓
LLM interpreta intenção
      ↓
Kernel seleciona função
      ↓
Plugin executa lógica
      ↓
LLM formata resposta
```

Esse pipeline pode incluir:

* múltiplas funções
* chamadas de ferramentas
* memória

---

# 9. Memory

Memory no Semantic Kernel permite armazenar **informações semânticas**.

Normalmente utiliza:

* embeddings
* banco vetorial

Fluxo de memória:

```
Texto
↓
Embedding
↓
Armazenar vetor
↓
Busca por similaridade
```

Isso permite que o sistema **lembre informações relevantes**.

---

# 10. Vector Databases

Bancos vetoriais armazenam embeddings.

Exemplos comuns:

* Pinecone
* Qdrant
* Weaviate
* Azure AI Search

Eles permitem:

```
Pergunta
↓
Gerar embedding
↓
Buscar vetores próximos
↓
Retornar documentos relevantes
```

Esse mecanismo é a base do **RAG**.

---

# 11. RAG (Retrieval Augmented Generation)

RAG significa **geração aumentada por recuperação**.

Fluxo:

```
Pergunta
↓
Busca vetorial
↓
Documentos relevantes
↓
LLM gera resposta
```

Isso permite que o modelo utilize **dados externos ao treinamento**.

---

# 12. Planning

Planning é a capacidade do sistema de **quebrar tarefas complexas em passos menores**.

Exemplo:

Usuário:

```
Crie um relatório dos clientes que mais compraram e envie por email.
```

Plano possível:

```
1. Buscar clientes
2. Calcular compras
3. Gerar relatório
4. Enviar email
```

O planner decide **quais funções chamar e em qual ordem**.

---

# 13. Agents

Agents são entidades autônomas baseadas em LLM.

Cada agente pode ter:

* objetivo
* ferramentas
* memória
* estratégia

Exemplo:

```
Agente pesquisador
Agente analista
Agente redator
```

Fluxo:

```
Research Agent
↓
Analysis Agent
↓
Writer Agent
```

Esse padrão permite **resolver tarefas complexas**.

---

# 14. Tool Calling

Tool Calling é quando o LLM decide **invocar funções do sistema**.

Exemplo:

Usuário pergunta:

```
Quantos pedidos tivemos hoje?
```

O modelo decide:

```
Chamar função:
ObterPedidosHoje()
```

Fluxo:

```
User
↓
LLM
↓
Tool Call
↓
Plugin
↓
Resultado
↓
LLM explica
```

---

# 15. Arquitetura de Aplicações com Semantic Kernel

Uma aplicação real normalmente segue essa estrutura:

```
Application Layer
   ↓
AI Orchestration (Semantic Kernel)
   ↓
LLM Services
   ↓
Plugins / Tools
   ↓
Databases / APIs
```

---

# 16. Boas práticas de engenharia

### 1. Não colocar lógica crítica no LLM

Errado:

```
LLM calcula valores financeiros
```

Certo:

```
Código calcula
LLM explica
```

---

### 2. Usar plugins para acessar dados

LLMs não devem:

* consultar banco diretamente
* executar queries complexas

Plugins fazem essa ponte.

---

### 3. Controlar prompts

Prompts são parte do software.

Devem ser:

* versionados
* testados
* documentados

---

### 4. Minimizar chamadas ao modelo

Cada chamada ao LLM:

* custa dinheiro
* aumenta latência

Projetar pipelines eficientes é essencial.

---

# 17. Modelo mental para projetar sistemas com SK

Sempre pense nestas perguntas:

1. O que o LLM precisa interpretar?
2. Que ferramentas o sistema precisa?
3. Onde os dados estão?
4. O que deve ser código vs IA?

---

# 18. Papel do Semantic Kernel

O Semantic Kernel transforma um LLM de:

```
Gerador de texto
```

em

```
Componente inteligente de um sistema
```

Ele permite criar aplicações como:

* assistentes corporativos
* automações inteligentes
* sistemas de análise
* copilots
* agentes autônomos

---

# Próximos tópicos importantes

* Prompt Engineering avançado
* Tool calling avançado
* RAG com Semantic Kernel
* Multi-agent systems
* Avaliação de modelos
* Guardrails de IA
