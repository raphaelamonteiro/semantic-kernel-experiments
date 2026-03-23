# AI Concepts for Engineers

Este documento reúne **conceitos fundamentais de Inteligência Artificial** necessários para construir aplicações modernas utilizando **LLMs, ferramentas e orquestração**.

O objetivo não é apenas entender os termos, mas **pensar como um engenheiro de IA**.

---

# 1. Modelos de Linguagem (LLMs)

## O que são LLMs

LLM significa **Large Language Model**.

São modelos treinados em grandes quantidades de texto para **prever o próximo token em uma sequência**.

Exemplo simplificado:

Entrada:

```
O céu é
```

O modelo prevê:

```
azul
```

Esse processo repetido gera frases completas.

---

## Exemplos de LLMs

Alguns modelos populares:

* GPT
* Gemini
* Llama
* Claude

Eles são usados para:

* geração de texto
* interpretação de linguagem natural
* raciocínio aproximado
* criação de código
* assistentes conversacionais

---

## Limitações dos LLMs

LLMs **não são sistemas completos**.

Eles:

❌ não acessam bancos de dados
❌ não executam código
❌ não conhecem seu sistema
❌ podem alucinar informações

Por isso, sistemas reais precisam de:

* ferramentas
* memória
* orquestração

---

# 2. Tokens

LLMs não trabalham diretamente com palavras.

Eles trabalham com **tokens**.

Tokens são pequenas unidades de texto.

Exemplo:

```
"Semantic Kernel é incrível"
```

Pode virar algo como:

```
["Semantic", "Kernel", "é", "incr", "ível"]
```

O modelo prevê **token por token**.

---

## Por que tokens importam

Tokens afetam:

* custo da API
* tamanho máximo de contexto
* desempenho

Exemplo aproximado:

```
1 token ≈ 4 caracteres em inglês
```

---

# 3. Prompt Engineering

Prompt é o **texto que você envia para o modelo**.

Exemplo:

```
Explique o que é computação quântica para uma criança.
```

O prompt define:

* contexto
* tarefa
* estilo de resposta

---

## Tipos de prompts

### Zero-shot

Sem exemplos.

```
Traduza para português:

Hello world
```

---

### Few-shot

Com exemplos.

```
Traduza para português:

Hello → Olá
Dog → Cachorro
Cat →
```

---

### Chain-of-thought

Incentiva o modelo a explicar o raciocínio.

```
Explique passo a passo como resolver este problema.
```

---

# 4. Embeddings

Embeddings são **representações numéricas de texto**.

Eles transformam texto em **vetores**.

Exemplo:

```
"gato" → [0.12, -0.44, 0.91, ...]
```

---

## Por que embeddings existem

Eles permitem medir **similaridade semântica**.

Exemplo:

```
gato
cachorro
leão
```

Todos ficam próximos no espaço vetorial porque são **animais**.

---

## Uso prático

Embeddings são usados em:

* busca semântica
* sistemas RAG
* clustering
* recomendação
* memória de IA

---

# 5. Busca Vetorial

Busca vetorial é uma forma de encontrar **conteúdo semanticamente semelhante**.

Fluxo:

```
Pergunta do usuário
↓
Gerar embedding
↓
Comparar com embeddings armazenados
↓
Retornar os mais próximos
```

Isso permite perguntas como:

```
"Como cancelar uma compra?"
```

Mesmo que o documento diga:

```
"Procedimento para reembolso"
```

---

# 6. RAG (Retrieval Augmented Generation)

RAG significa **Retrieval Augmented Generation**.

É uma técnica que permite ao modelo **usar dados externos**.

Fluxo:

```
Pergunta do usuário
↓
Busca em base de conhecimento
↓
Documentos relevantes
↓
LLM gera resposta usando esses documentos
```

Isso resolve um grande problema dos LLMs:

**dados desatualizados ou inexistentes no treinamento**.

---

## Exemplo

Pergunta:

```
Qual é a política de reembolso da empresa?
```

Pipeline:

```
Pergunta
↓
Busca vetorial
↓
Documento da política
↓
LLM gera resposta baseada no documento
```

---

# 7. Ferramentas (Tools)

Ferramentas são **funções externas que o LLM pode usar**.

Exemplo:

```
ConsultarPedidos()
EnviarEmail()
BuscarCliente()
```

O modelo decide **quando usar essas ferramentas**.

---

## Exemplo de fluxo

```
User: Quantos pedidos tivemos hoje?

LLM:
Preciso consultar o sistema

Tool:
ObterPedidosHoje()

Resultado:
152 pedidos

LLM responde ao usuário.
```

---

# 8. Plugins

Plugins são **coleções organizadas de ferramentas**.

Exemplo:

```
PedidosPlugin
  ├── ObterPedidosHoje
  ├── ListarPedidos
  └── CalcularFaturamento
```

No **Semantic Kernel**, plugins expõem funções para o LLM.

---

# 9. Orquestração de IA

Orquestração é o processo de **controlar como IA e sistemas trabalham juntos**.

Ela coordena:

* prompts
* chamadas de ferramentas
* memória
* respostas

---

## Exemplo de pipeline

```
User pergunta
↓
LLM interpreta intenção
↓
Sistema decide ferramenta
↓
Ferramenta executa lógica
↓
LLM gera resposta final
```

Frameworks como **Semantic Kernel** fazem essa orquestração.

---

# 10. Agentes de IA

Agentes são sistemas baseados em LLM que podem:

* tomar decisões
* executar tarefas
* usar ferramentas
* colaborar com outros agentes

---

## Exemplo de sistema multi-agente

```
Agente Pesquisador
↓
Agente Analista
↓
Agente Escritor
```

Cada agente possui:

* papel
* ferramentas
* objetivo

---

# 11. Alucinação de IA

Alucinação ocorre quando o modelo **inventa informações incorretas**.

Exemplo:

Pergunta:

```
Quem fundou a empresa X em 1992?
```

Se o modelo não souber, ele pode **inventar um nome plausível**.

---

## Como reduzir alucinação

Boas práticas:

* usar RAG
* limitar contexto
* validar com código
* usar ferramentas

---

# 12. Princípio Fundamental da Engenharia de IA

Uma regra prática muito importante:

```
LLM = linguagem e interpretação
Código = lógica e dados
```

---

## Exemplo correto

```
Código:
calcula relatório financeiro

LLM:
explica o relatório
```

---

## Exemplo incorreto

```
LLM:
calcula relatório financeiro
```

Isso gera:

* inconsistência
* erros
* custo alto

---

# 13. Arquitetura de Aplicações com IA

Arquitetura moderna:

```
User
↓
Application
↓
AI Orchestration Layer
↓
LLM
↓
Tools / APIs / Databases
```

Frameworks de orquestração:

* Semantic Kernel
* LangChain
* LlamaIndex

---

# 14. Componentes de um Sistema de IA Moderno

Um sistema típico inclui:

```
LLM
Embeddings
Vector Database
Tools
Orchestration
Application Layer
```

---

# 15. Mentalidade de Engenheiro de IA

Um engenheiro de IA precisa pensar em:

* arquitetura
* custo de inferência
* latência
* confiabilidade
* integração com sistemas existentes

IA não substitui software tradicional.

Ela **amplia o que software pode fazer**.

---

# Próximos conceitos a estudar

* Prompt Templates
* Tool Calling
* AI Agents
* Memory Systems
* Semantic Kernel Architecture
* AI Evaluation
* Guardrails



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


# modelId: "qwen2.5:3b"

👉 modelo de 3B parâmetros

- é fraco pra function calling
- se perde fácil em instruções longas
- ignora regras complexas