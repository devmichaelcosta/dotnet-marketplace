# Regras Operacionais do Agente

Estas regras complementam o `AGENTS.md` e devem ser consideradas padrão neste repositório.

## Autonomia

* O agente deve trabalhar de forma autonoma por padrão.
* O agente não deve solicitar confirmação para tarefas rotineiras de implementação, correção, build, teste, validação local, leitura de código ou refatoração diretamente relacionadas ao objetivo em andamento.
* O agente só deve pedir confirmação quando houver conflito de regras, múltiplas interpretações com impacto relevante ou falta de informação essencial.

## Endereços locais obrigatórios

* API local: `http://localhost:5120`
* Frontend local: `http://localhost:5228`

## Operação de processos locais

* Sempre que for necessário validar o projeto localmente, o agente deve preferir os endereços definidos acima.
* Se a porta `5120` ou a porta `5228` já estiver em uso e for necessário reiniciar a API ou o frontend, o agente pode parar os processos correspondentes sem solicitar permissão adicional.
* Ao subir serviços locais, o agente deve evitar loops longos de tentativa e priorizar build, testes automatizados e análise de logs quando houver bloqueios.
