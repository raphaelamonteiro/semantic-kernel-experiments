## 1️⃣ O que é o Semantic Kernel

O **Semantic Kernel** é um framework para construir **apps de IA generativa** ou **agentes de IA** combinando:

* LLMs (OpenAI, Azure OpenAI, HuggingFace, etc.)
* código normal (C#, Python, Java)
* plugins / tools
* memória (embeddings)
* planejamento de tarefas

Ele permite criar sistemas onde a IA **decide quais funções executar** para resolver um objetivo. ([GitHub][1])

Exemplo:

* Usuário: *“Reserve um hotel e mande o email”*
* O kernel cria um plano e chama:

  * API de hotel
  * função de email
  * banco de dados

---

# 2️⃣ O que é um repo **semantic-kernel-experiments**

Esse tipo de repositório geralmente contém **experimentos com o Semantic Kernel**, como:

### 🧪 Testes de features

* Agents
* planners
* tool calling
* plugins
* RAG

### 🧪 Prototipagem

Antes de virar código oficial.

Exemplo de experimentos comuns:

* multi-agent systems
* chat com documentos (RAG)
* integração com APIs
* pipelines de automação
* memory embeddings

---

# 3️⃣ Estrutura típica desse tipo de repo

Normalmente você vai ver algo assim:

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

# 4️⃣ Exemplo de experimento simples

```python
from semantic_kernel import Kernel
from semantic_kernel.connectors.ai.open_ai import OpenAIChatCompletion

kernel = Kernel()

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

Esse experimento testa apenas **LLM integration**.

---

# 5️⃣ Para que servem esses experiments

Eles ajudam a:

* testar **novas arquiteturas de agentes**
* estudar **RAG pipelines**
* validar **integração com LLMs**
* explorar **automação com IA**

Ou seja:
👉 são **POCs (proof of concept)**.

---

✅ **Resumo**

`semantic-kernel-experiments` é um repo usado para **experimentar ideias com Semantic Kernel**, como:

* AI agents
* RAG
* plugins
* tool calling
* automação com LLMs

Não é necessariamente produção — é mais **pesquisa e protótipos**.

---

💡 Se quiser, posso também te mostrar:

* um **exemplo real de semantic-kernel-experiments no GitHub**
* um **mini projeto de AI agent com Semantic Kernel**
* ou comparar **Semantic Kernel vs LangChain vs LangGraph** (isso ajuda muito a entender).

[1]: https://github.com/microsoft/semantic-kernel?utm_source=chatgpt.com "GitHub - microsoft/semantic-kernel: Integrate cutting-edge LLM technology quickly and easily into your apps"
