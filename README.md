#  🧪 Semantic Kernel Experiments

Repositório dedicado ao estudo prático e aprofundado do **Semantic Kernel** da Microsoft usando **C#**.

O objetivo deste projeto é desenvolver **autonomia real na construção de aplicações de IA**, entendendo não apenas *como usar*, mas **como projetar sistemas com LLMs**.

Este repositório documenta experimentos, conceitos e arquiteturas relacionadas à **orquestração de IA**.

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
* **Ollama** para execução local de modelos
* Modelo **Qwen**

---

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

O Semantic Kernel atua na camada de **orquestração**.

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
* **Qwen**

⚠️ Importante: LLMs **não executam código nem acessam sistemas diretamente**.

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

# 📂 Estrutura do Repositório

semantic-kernel-experiments
│
├─ Plugins
│   ├─ LightModel.cs
│   └─ LightsPlugin.cs
│
└─ experiments
    ├─ basic-plugin
    │   └─ exemplo simples baseado na documentação
    │
    └─ interactive-chat
        └─ chat interativo que permite controlar as luzes

---

# ⚙️ Tecnologias Utilizadas

* .NET 10 ou superior
* C#
* RestSharp
* **Semantic Kernel**
* **Ollama**
* **Qwen**

---

# ▶️ Como Executar o Projeto

## 1 — Instalar .NET

Instale o .NET SDK:

[https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download)

Verifique:

```bash
dotnet --version
```

---

# 2 — Instalar Ollama

Instale o Ollama:

[https://ollama.com/download](https://ollama.com/download)

Após instalar, verifique:

```bash
ollama --version
```

---

# 3 — Baixar o modelo

Este projeto utiliza o modelo **Qwen**.

Execute:

```bash
ollama pull qwen2.5:7b
```

---

# 4 — Clonar o repositório

```bash
git clone https://github.com/raphaelamonteiro/semantic-kernel-experiments.git
```

Entrar na pasta:

```bash
cd semantic-kernel-experiments
```

---

# 5 — Restaurar pacotes

```bash
dotnet restore
```

# 6 — Executar a aplicação

```bash
dotnet run --project experiments/interactive-chat
ou
dotnet run --project experiments/basic-plugin
```

---



# Importante:
⚠️ Repositório criado para fins de **pesquisa e prototipagem**.

---

# 📚 Referências

* Documentação oficial do Semantic Kernel
* Arquiteturas modernas de aplicações com LLM
* Práticas de engenharia de IA
