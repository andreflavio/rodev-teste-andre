import os

def listar_estrutura(diretorio, arquivo, nivel=0, excluir_pastas=("bin", "obj", ".git"), max_depth=10):
    """Lista a estrutura de pastas e arquivos de um diretório, excluindo pastas especificadas e com profundidade máxima."""
    if nivel >= max_depth:
        return

    try:
        itens = sorted(os.listdir(diretorio))
        pastas = [item for item in itens if os.path.isdir(os.path.join(diretorio, item)) and item not in excluir_pastas]
        arquivos = [item for item in itens if os.path.isfile(os.path.join(diretorio, item))]

        for pasta in pastas:
            arquivo.write("  " * nivel + f"[{pasta}]\n")
            listar_estrutura(os.path.join(diretorio, pasta), arquivo, nivel + 1, excluir_pastas, max_depth)

        # Listar arquivos até uma profundidade razoável
        if nivel < 5:
            for arquivo_nome in arquivos:
                arquivo.write("  " * nivel + f"[Arquivo] {arquivo_nome}\n")

    except OSError as e:
        print(f"Erro ao acessar o diretório '{diretorio}': {e}")

if __name__ == "__main__":
    diretorio_raiz = "."  # Use "." para o diretório onde o script está sendo executado
    nome_arquivo_saida = "estrutura_projeto.txt"
    max_profundidade = 10  # Define a profundidade máxima para listar as pastas

    try:
        with open(nome_arquivo_saida, "w", encoding="utf-8") as arquivo_saida:
            arquivo_saida.write(f"Estrutura de Pastas do Projeto: {os.path.abspath(diretorio_raiz)}\n")
            arquivo_saida.write("-" * 40 + "\n")
            listar_estrutura(diretorio_raiz, arquivo_saida, 0, ("bin", "obj", ".git"), max_profundidade)
        print(f"Estrutura do projeto gerada com sucesso e salva em '{nome_arquivo_saida}' (profundidade máxima: {max_profundidade})")
    except IOError as e:
        print(f"Erro ao criar ou escrever no arquivo '{nome_arquivo_saida}': {e}")