# Regras Operacionais do Agente

Estas regras complementam o `AGENTS.md` e devem ser consideradas padrao neste repositorio.

## Autonomia

* O agente deve trabalhar de forma autonoma por padrao.
* O agente nao deve solicitar confirmacao para tarefas rotineiras de implementacao, correcao, build, teste, validacao local, leitura de codigo ou refatoracao diretamente relacionadas ao objetivo em andamento.
* O agente so deve pedir confirmacao quando houver conflito de regras, multiplas interpretacoes com impacto relevante ou falta de informacao essencial.

## Enderecos locais obrigatorios

* API local: `http://localhost:5120`
* Frontend local: `http://localhost:5228`

## Operacao de processos locais

* Sempre que for necessario validar o projeto localmente, o agente deve preferir os enderecos definidos acima.
* Se a porta `5120` ou a porta `5228` ja estiver em uso e for necessario reiniciar a API ou o frontend, o agente pode parar os processos correspondentes sem solicitar permissao adicional.
* Ao subir servicos locais, o agente deve evitar loops longos de tentativa e priorizar build, testes automatizados e analise de logs quando houver bloqueios.
* Sempre que o agente levantar servidores locais para validacao, deve encerra-los antes de finalizar a tarefa.
* O agente nao deve deixar processos da API ou do frontend executando em segundo plano ao concluir uma entrega, salvo se o usuario pedir explicitamente para mante-los ativos.

## Regra de encoding

* Ao editar arquivos existentes, o agente deve preservar o encoding atual do arquivo sempre que possivel.
* Ao criar novos arquivos de codigo, configuracao, teste ou documentacao, o agente deve preferir UTF-8 consistente com o repositorio e evitar trocar encoding sem necessidade.
* O agente nao deve recriar arquivos textuais em outro encoding apenas para pequenas alteracoes.
* Se houver sinais de texto corrompido, o agente deve tratar isso como problema de compatibilidade de encoding e revisar a estrategia antes de continuar editando em lote.
