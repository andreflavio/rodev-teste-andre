import secrets
import base64

# Gera 64 bytes aleatórios (512 bits)
random_bytes = secrets.token_bytes(64)

# Codifica para Base64 para obter uma string imprimível
secret_key = base64.b64encode(random_bytes).decode('utf-8')

print(secret_key)