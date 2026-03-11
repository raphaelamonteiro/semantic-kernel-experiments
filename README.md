Semantic Kernel é um SDK Open Source desenvolvido pela Microsoft e comunidade que tem como objetivo **ajudar a criar agentes para acionar códigos existentes.**
Podendo utilizar modelos existentes como OpenAI, Hugging Face e Azure OpenAI.


Ele é utilizado para ser o orquestrador das chamadas. Seu nome é em homenagem ao Unix/Kernel que lhe dá a possibilidade de encadear chamadas através do uso do pipe:


```python
fazer-algo | outra-coisa | gravar-resultado >> nome-arquivo.txt  
```
   
> Seu papel é ser o facilitador para integrar as chamadas dos códigos existentes estendendo para criação de memórias e acesso aos plugins, reduzindo a necessidade de escrever diversos trechos de código, sendo seu braço e suas mãos de sua aplicação

## 1️⃣ O que é o Semantic Kernel

O **Semantic Kernel** é um framework para construir **aplicações de IA generativa** ou **agentes de IA** combinando:

* LLMs (OpenAI, Azure OpenAI, HuggingFace, etc.)
* código tradicional (C#, Python, Java)
* plugins / tools
* memória (embeddings)
* planejamento de tarefas

Key features and components include:
AI Orchestration: Allows chaining of prompts and plugins to create complex, automated workflows.
Plugins & Skills: Connects AI to external data and systems, allowing models to execute native code.
Memory & Vector Databases: Supports Retrieval Augmented Generation (RAG) to provide context-aware responses.
Multi-Language Support: Compatible with C#, Python, and Java.
Planners: Automatically generates plans to fulfill user requests using available plugins. 


Ele permite criar sistemas onde a IA **decide quais funções executar** para atingir um objetivo.
Mais detalhes no [GitHub do Semantic Kernel][1].

**Exemplo:**

* Usuário: *“Reserve um hotel e envie o email”*
* O kernel cria um plano e chama:

  * API de hotel
  * função de email
  * banco de dados

---

## 2️⃣ O que é um repo `semantic-kernel-experiments`

Este tipo de repositório contém **experimentos com o Semantic Kernel**, como:

### 🧪 Testes de features

* Agents
* Planners
* Tool calling
* Plugins
* RAG (Retrieval-Augmented Generation)

### 🧪 Prototipagem

Antes de virar código oficial.

**Exemplos comuns de experimentos:**

* Multi-agent systems
* Chat com documentos (RAG)
* Integração com APIs
* Pipelines de automação
* Memory embeddings

---

## 3️⃣ Estrutura típica de um repo

Normalmente você verá algo assim:

```
semantic-kernel-experiments
 ├─ agents/
 │   ├─ planner-agent
 │   └─ tool-agent
 ├─ rag/
 │   ├─ vector-db
 │   └─ document-chat
 ├─ plugins/
 │   ├─ weather-plugin
 │   └─ email-plugin
 ├─ notebooks/
 └─ examples/
```

Cada pasta testa **uma ideia diferente**.

---

## 4️⃣ Exemplo de experimento simples (Python)

```python
from semantic_kernel import Kernel
from semantic_kernel.connectors.ai.open_ai import OpenAIChatCompletion

# Cria o kernel
kernel = Kernel()

# Adiciona serviço de chat
kernel.add_service(
    OpenAIChatCompletion(
        service_id="chat",
        api_key="API_KEY",
        model="gpt-4"
    )
)

prompt = "Explain Semantic Kernel in simple words"

result = kernel.invoke_prompt(prompt)
print(result)
```

Esse experimento testa apenas **integração com LLMs**.

---

## 5️⃣ Para que servem esses experiments

Eles ajudam a:

* testar **novas arquiteturas de agentes**
* estudar **pipelines de RAG**
* validar **integração com LLMs**
* explorar **automação com IA**

👉 Em resumo, são **POCs (proof of concept)**.

---

## ✅ Resumo

`semantic-kernel-experiments` é um repositório usado para **experimentar ideias com Semantic Kernel**, como:

* AI agents
* RAG
* Plugins
* Tool calling
* Automação com LLMs

⚠️ Não é necessariamente código de produção — é mais para **pesquisa e prototipagem**.

---

[1]: https://github.com/microsoft/semantic-kernel "Semantic Kernel GitHub"


