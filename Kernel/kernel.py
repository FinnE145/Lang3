from subprocess import Popen, PIPE

exe_path = 'C:\\Users\\finne\\OneDrive\\Documents\\0coding\\Lang3\\Lang3\\bin\\Debug\\net7.0\\Lang3.exe'
process = Popen([exe_path, "abc", "defg"], stdout=PIPE)

lc = 0
while True:
    line = process.stdout.readline()
    lc += 1
    if not line:
        break
    print(f"Line {lc}: {line.decode('utf-8').strip()}")