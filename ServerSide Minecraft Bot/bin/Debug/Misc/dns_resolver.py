import dns.resolver
import socket

def lookup(addr):
    host = addr
    port = None
    if ':' in addr:
        x = addr.split(':')
        if len(x) > 2:
            raise ValueError(f'Invalid address: \'{addr}\'')
        host = x[0]
        port = int(x[1])
    if not port:
        port = 25565
        try:
            answers = dns.resolver.resolve('_minecraft._tcp.' + host, 'SRV')
            if len(answers):
                answer = answers[0]
                host = str(answer.target).rstrip('.')
                port = int(answer.port)
        except Exception:
            pass
    return (host, socket.gethostbyname(host), port)

print(lookup(input('Server: ')))
