# Objeto de Transferência de Dados (DTO)
Quase nem sempre é bom que o cliente da Api tenha acesso a todas as informações referentes as classes, pensando nisso o DTO é usado para:
-   Remova as referências circulares.
	Exemplo: Usuário possui uma imagem que pertence a um usúario que possui uma imagem que per... 
-   Oculte propriedades específicas que os clientes não devem exibir.
	Exemplo: Retornar a entidade usuário mas sem mostrar os campos id, senha, email. 	
-   Omita algumas propriedades para reduzir o tamanho da carga. (Desempenho)
-   Nivelar grafos de objeto que contêm objetos aninhados, para torná-los mais convenientes para os clientes.
-   Validação de dados de entrada.
	Exemplo: 
	Para login pedir somente uma entidade com os campos: email e senha.
	Ao serem validados essa entidade pode ser mapeada e transformada em um objeto da classe usuário.

Classe usuário:
```json
{
   "Nome": "usuario",
   "Celular": "4299999999",
   "NumeroDeSerieEscritorio": "999999",
   "Email": "pessoa@mail.com",
   "Senha": "123456789",
   "ConfirmacaoSenha": "123456789"
}
```
Dto usuário (Login):
```json
{
   "NumeroDeSerieEscritorio": "999999",
   "Email": "pessoa@mail.com",
   "Senha": "123456789"
}
```
