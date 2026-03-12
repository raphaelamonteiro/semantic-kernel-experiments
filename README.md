# Semantic Kernel Experiments

Repositório dedicado ao estudo prático e aprofundado do **Semantic Kernel** da Microsoft usando **C#**.

O objetivo deste projeto é desenvolver **autonomia real na construção de aplicações de IA**, entendendo não apenas *como usar*, mas **como projetar sistemas com LLMs**.

Este repositório documenta experimentos, conceitos e arquiteturas relacionadas à orquestração de IA.

---

# 🎯 Objetivo

Aprender a construir aplicações modernas de IA utilizando:

* LLMs
* Orquestração de ferramentas
* Memória vetorial
* Plugins
* Agentes

Utilizando principalmente:

* **C# / .NET**
* **Semantic Kernel**
* APIs de modelos de linguagem


# 🧠 O que é o Semantic Kernel

O **Semantic Kernel** é um framework de **orquestração de IA** criado pela Microsoft.

Ele permite integrar **modelos de linguagem (LLMs)** com:

* código tradicional
* APIs
* bancos de dados
* memória vetorial
* ferramentas externas

Arquitetura simplificada:

```
Application
     ↓
Semantic Kernel
     ↓
LLM + Plugins + Memory + Agents
```

O objetivo é transformar modelos de linguagem em **componentes utilizáveis dentro de sistemas reais**.

---

# 🏗 Arquitetura Moderna de Aplicações com LLM

Uma aplicação com IA normalmente segue essa estrutura:

```
User
 ↓
Application Layer
 ↓
AI Orchestration Layer
 ↓
LLM Provider
 ↓
Tools / APIs / Databases
```

O **Semantic Kernel atua na camada de orquestração**.

Ele controla:

* chamadas ao LLM
* uso de ferramentas
* memória
* execução de fluxos

---

# 📚 Conceitos Fundamentais

## LLM (Large Language Model)

Modelos treinados em grandes volumes de texto capazes de:

* gerar texto
* interpretar linguagem natural
* realizar raciocínio aproximado

Exemplos:

* GPT
* Gemini
* Llama

>⚠️ Importante: LLMs **não executam código nem acessam sistemas diretamente**.

---

## Plugins

Plugins são **funções do código que o LLM pode utilizar como ferramentas**.

Exemplo conceitual:

```
User: "Quantos pedidos tivemos hoje?"

LLM decide usar:

PedidosPlugin.ObterPedidosHoje()
```

O LLM interpreta a intenção e usa a função correta.

---

## Orquestração

Orquestração é o **controle do fluxo entre IA, código e ferramentas**.

Exemplo:

```
User pergunta
      ↓
LLM interpreta intenção
      ↓
Kernel decide ferramenta
      ↓
Plugin executa lógica
      ↓
LLM gera resposta
```

O **Kernel controla esse fluxo**.

---

## Embeddings

Embeddings são **vetores numéricos que representam significado de texto**.

Exemplo:

```
"gato" → [0.21, -0.84, 0.13 ...]
"cachorro" → [0.19, -0.80, 0.15 ...]
```

Textos com significados parecidos ficam **próximos no espaço vetorial**.

Isso permite:

* busca semântica
* RAG
* memória de IA

---

## Agents

Agentes são **entidades autônomas baseadas em LLMs** capazes de:

* decidir ações
* usar ferramentas
* colaborar com outros agentes

Exemplo:

```
Agente pesquisador
Agente analista
Agente redator
```

---

# ⚙️ Filosofia de Engenharia de IA

Uma regra fundamental ao construir sistemas com LLMs:

```
LLM → interpretação e linguagem
Código → lógica e dados
```

❌ Errado

```
LLM calcula relatório financeiro
```

✔️ Certo

```
Código calcula
LLM explica
```

Motivos:

* LLMs não são determinísticos
* custo de inferência
* possibilidade de alucinação

---

<!---
# 🧪 Estrutura do Repositório

```
semantic-kernel-experiments
│
├── 01-fundamentals
│   Conceitos básicos
│
├── 02-first-kernel
│   Primeiro projeto com Semantic Kernel
│
├── 03-plugins
│   Criação de plugins em C#
│
├── 04-memory
│   Experimentos com embeddings
│
├── 05-rag
│   Retrieval Augmented Generation
│
├── 06-agents
│   Sistemas multi-agentes
│
└── projects
    Projetos completos
```

---

# 🧪 Experimentos Planejados

## Projeto 1 — CLI AI Assistant

Um assistente de terminal usando LLM.

Conceitos:

* Kernel
* Chat completion
* Prompt templates

---

## Projeto 2 — Plugins e Tools

LLM chamando funções C#.

Conceitos:

* Kernel Functions
* Plugins
* Tool calling

---

## Projeto 3 — RAG

Sistema de perguntas e respostas baseado em documentos.

Conceitos:

* embeddings
* vector search
* memory

---

## Projeto 4 — AI API

API com IA integrada.

Conceitos:

* ASP.NET
* integração com Kernel
* pipelines

---

## Projeto 5 — Multi-Agent System

Sistema com múltiplos agentes colaborando.

Conceitos:

* agents
* planning
* task delegation

---
-->

# 🧠 Aprendizados Importantes

Alguns princípios importantes da engenharia de IA:

### 1 — LLMs não são bancos de dados

Dados críticos devem estar em **bases estruturadas**.

---

### 2 — LLMs não devem executar lógica crítica

Use código tradicional para:
* cálculos
* regras de negócio
* validações

---

### 3 — Ferramentas tornam a IA poderosa

Sem ferramentas:

```
LLM = chatbot
```

Com ferramentas:

```
LLM = sistema inteligente
```

---
<!---
# 🚀 Próximos Passos

* Criar primeiro projeto com Semantic Kernel
* Conectar com API de LLM
* Criar primeiro plugin
* Construir pipeline de execução
-->
---


# 🧠 Filosofia deste repositório

Este projeto não é apenas sobre **usar IA**, mas sobre **entender como projetar sistemas de IA de forma profissional**.

O objetivo final é desenvolver autonomia para construir aplicações inteligentes sem depender de tutoriais passo a passo.


>⚠️ Repositório criado para fins de **pesquisa e prototipagem**.

---

[1]: https://github.com/microsoft/semantic-kernel "Semantic Kernel GitHub"

# 📚 Referências

* Documentação oficial do Semantic Kernel
* Arquiteturas modernas de aplicações com LLM
* Práticas de engenharia de IA

---

